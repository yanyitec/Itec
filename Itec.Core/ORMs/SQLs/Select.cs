
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

       

        Func<IDataReader, T> _Fill;


        public T FillEntity(IDataReader reader)
        {
            if (_Fill == null)
            {
                lock (this)
                {
                    if (_Fill == null) _Fill = GenFill();
                }
            }
            return _Fill(reader);
        }

        public List<T> Execute(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, conn, trans);

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

        public async Task<List<T>> ExecuteAsync(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, conn, trans);

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




        public virtual DbCommand BuildCommand(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            var sql = GetSql(expr, cmd);
            cmd.CommandText = sql;
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),sql);
            return cmd;
        }

        string _tbAndFields;
        string _tbAndFieldsWithWhere;

        protected string BuildTbAndFieldsSql() {
            var fields = string.Empty;
            foreach (var pair in this.Sql.AllowedProps)
            {
                var prop = pair.Value;
                var fieldname = this.Sql.DbTrait.SqlFieldname(prop.Field.Name);
                if (fields != string.Empty) fields += ",";
                fields += fieldname;
            }
            _tbAndFields = $"SELECT {fields} FROM {this.Sql.Tablename(true)} ";
            _tbAndFieldsWithWhere = _tbAndFields + " WHERE ";

            return _tbAndFieldsWithWhere;
        }

        protected string GetSql(Expression expr, DbCommand cmd)
        {
            if (_tbAndFieldsWithWhere == null)
            {
                lock (this)
                {
                    if (_tbAndFieldsWithWhere == null)  this.BuildTbAndFieldsSql();
                }
            }
            
            if (expr != null)
            {
                IDbProperty prop = null;
                var cond = this.SqlWhere(expr, cmd,ref prop,0);
                if (string.IsNullOrEmpty(cond)) return _tbAndFields;
                var sql = _tbAndFieldsWithWhere + cond;
                return sql;
            }
            else return _tbAndFields;


        }
        static readonly MethodInfo[] DataReaderMethodInfos = typeof(IDataRecord).GetMethods();
        //static readonly MethodInfo DataReaderGetItemMethod = typeof(IDataReader).GetMethod("get_Item", new Type[] { typeof(int) });
        Func<IDataReader,T> GenFill()
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

            var block = Expression.Block(new List<ParameterExpression> {indexExpr, valObjExpr, entityExpr }, codes);
            //IDataReader rs;rs[]
            var lamda = Expression.Lambda<Func<IDataReader,T>>(block, readerExpr);
            return lamda.Compile();
        }
    }
}
