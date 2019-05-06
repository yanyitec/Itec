
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs.SQLs
{
    public class Select<T>:Where<T>
    {
        public Select(SQL<T> sql):base(sql)
        {
            
        }

       

        Func<IDataReader,int, T> _Fill;


        public T FillEntity(IDataReader reader,int at=0)
        {
            if (_Fill == null)
            {
                lock (this)
                {
                    if (_Fill == null) _Fill = GenFill();
                }
            }
            return _Fill(reader,at);
        }

        public List<T> Execute(IFilterable<T> filterable, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(filterable, conn, trans);

            var result = new List<T>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var entity = FillEntity(reader);
                    result.Add(entity);
                }
            }
            return result;
        }

        public async Task<List<T>> ExecuteAsync(IFilterable<T> filterable, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(filterable, conn, trans);

            var result = new List<T>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var entity = FillEntity(reader);
                    result.Add(entity);
                }
            }
            return result;
        }




        public virtual DbCommand BuildCommand(IFilterable<T> filterable, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            var sql = GetSql(filterable, cmd);
            cmd.CommandText = sql;
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),"SQL executing:[{0}]",sql);
            return cmd;
        }

        

        protected string GetSql(IFilterable<T> filterable, DbCommand cmd)
        {
            
            var expr = filterable.QueryExpression;
            var wOpts = new WhereOpts();
            string where=null;
            string orderBy=null;
            if (filterable.QueryExpression != null) {
                where = this.SqlWhere(expr, cmd,wOpts);
            }
            if (filterable.AscendingExpression != null) {
                orderBy = this.Sql.DbTrait.SqlFieldname(this.SqlWhere((filterable.AscendingExpression.Body as UnaryExpression).Operand, cmd, wOpts)) + " ASC";
            }
            else if (filterable.DescendingExpression != null)
            {
                orderBy = this.Sql.DbTrait.SqlFieldname(this.SqlWhere((filterable.DescendingExpression.Body as UnaryExpression).Operand, cmd, wOpts)) + " DESC";
            }
            if(where==null && orderBy==null && filterable.SkipCount<-0 && filterable.TakeCount<=0)  return this.Sql.SqlTableAndFields;
            return this.Sql.DbTrait.SqlPaginate(
                this.Sql.Tablename(true),
                this.Sql.SqlFields,
                where,
                orderBy,
                filterable.TakeCount,
                filterable.SkipCount
            );

        }
        static readonly MethodInfo[] DataReaderMethodInfos = typeof(IDataRecord).GetMethods();
        //static readonly MethodInfo DataReaderGetItemMethod = typeof(IDataReader).GetMethod("get_Item", new Type[] { typeof(int) });
        Func<IDataReader,int,T> GenFill()
        {
            var getItemMethodInfo = DataReaderMethodInfos.FirstOrDefault(p=>p.Name=="get_Item" && p.GetParameters()[0].ParameterType == typeof(int));

            var readerExpr = Expression.Parameter(typeof(IDataReader), "reader");
            var indexExpr = Expression.Parameter(typeof(int), "index");
            var valObjExpr = Expression.Parameter(typeof(object), "valObj");
            var entityExpr = Expression.Parameter(typeof(T), "entity");

            
            var codes = new List<Expression>();
            codes.Add(Expression.Assign(indexExpr,Expression.Constant(0)));
            codes.Add(Expression.Assign(entityExpr, Expression.New(typeof(T))));
            foreach (var pair in this.Sql.AllowedProps)
            {
                var fieldname = Sql.DbTrait.SqlFieldname(pair.Key);
                var member = pair.Value;
                var readReaderExpr = Expression.Assign(
                    valObjExpr,
                    Expression.Call(readerExpr, getItemMethodInfo, Expression.PostIncrementAssign(indexExpr))
                );
                codes.Add(readReaderExpr);
                Expression convertExpr = null;
                if (member.Nullable)
                {
                    var ctorMethod = member.PropertyType.GetConstructors().First(p => p.GetParameters().Length == 1);

                    convertExpr = Expression.New(ctorMethod,
                        Expression.Convert(valObjExpr, member.NonullableType)
                    );
                }
                else
                {
                    convertExpr = Expression.Convert(valObjExpr, member.NonullableType);
                }

                var chkDbNullExpr = Expression.IfThen(
                 Expression.NotEqual(valObjExpr, Expression.Constant(DBNull.Value))
                 , Expression.Assign(
                    Expression.PropertyOrField(entityExpr, member.Name)
                    , convertExpr
                 )
                );
                codes.Add(chkDbNullExpr);
            }
            var retLabel = Expression.Label(typeof(T));
            codes.Add(Expression.Return(retLabel, entityExpr));
            codes.Add(Expression.Label(retLabel,entityExpr));

            var block = Expression.Block(new List<ParameterExpression> {valObjExpr, entityExpr }, codes);
            //IDataReader rs;rs[]
            var lamda = Expression.Lambda<Func<IDataReader,int,T>>(block, readerExpr,indexExpr);
            return lamda.Compile();
        }
    }
}
