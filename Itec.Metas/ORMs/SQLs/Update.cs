//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Itec.ORMs.SQLs
//{
//    public class Update
//    {
//        public Update(EntityClass model, string fieldnames)
//        {
//            this.Model = model;
//            this.DbSettings = model.Database.Settings;
//            this.Trait = model.Database.Trait;
//            //this.Fieldnames = fieldnames;
//        }

//        public EntityClass Model { get; private set; }


//        public DbSettings DbSettings { get; private set; }

//        public DbTrait Trait { get; private set; }

//        Func<IDataReader, object> _Fill;


//        protected object FillEntity(IDataReader reader)
//        {
//            if (_Fill == null)
//            {
//                lock (this)
//                {
//                    if (_Fill == null) _Fill = GenFill() as Func<IDataReader, object>;
//                }
//            }
//            return _Fill(reader);
//        }

//        public int Execute(Expression expr,object entity, DbConnection conn, DbTransaction trans, Action<object> readed = null)
//        {

//            var cmd = BuildCommand(expr,entity, conn, trans);
//            int i = 0;
//            using (var reader = cmd.ExecuteReader())
//            {
//                while (reader.Read())
//                {
//                    var entity1 = FillEntity(reader);
//                    readed(entity1);
//                    i++;
//                }
//            }
//            return i;
//        }

//        public async Task<int> ExecuteAsync(Expression expr, object entity, DbConnection conn, DbTransaction trans, Action<object> readed = null)
//        {
//            var cmd = BuildCommand(expr,entity, conn, trans);
//            int i = 0;
//            using (var reader = await cmd.ExecuteReaderAsync())
//            {
//                while (await reader.ReadAsync())
//                {
//                    var entity1 = FillEntity(reader);
//                    readed(entity1);
//                    i++;
//                }
//            }
//            return i;
//        }

//        public virtual DbCommand BuildCommand(Expression expr,object entity, DbConnection conn, DbTransaction trans)
//        {

//            var cmd = this.Trait.CreateDbCommand(conn,null);
//            cmd.Connection = conn;
//            cmd.Transaction = trans;
//            cmd.CommandType = System.Data.CommandType.Text;
//            var sql = GetSql(expr, cmd);
//            cmd.CommandText = sql;

//            return cmd;
//        }

//        string _tbAndFields;
//        string _tbAndFieldsWithWhere;

//        protected string GetSql(Expression expr, DbCommand cmd)
//        {
//            if (_tbAndFields == null)
//            {
//                lock (this)
//                {
//                    if (_tbAndFields == null)
//                    {
//                        var fields = string.Empty;
//                        foreach (var member in this.Model.Members)
//                        {
//                            var fieldname = this.Trait.MakeSqlFieldname(member);
//                            if (fieldname == null) continue;
//                            if (fields != string.Empty) fields += ",";
//                            fields += fieldname;
//                        }
//                        _tbAndFields = $"SELECT {fields} FROM {this.Trait.MakeSqlTablename(this.Model)} ";
//                        _tbAndFieldsWithWhere += " WHERE";
//                    }
//                }
//            }
//            if (expr == null)
//            {
//                var where = new Where(this.Model);
//                var cond = where.BuildSql(expr, cmd);
//                if (string.IsNullOrEmpty(cond)) return _tbAndFields;
//                var sql = _tbAndFieldsWithWhere + cond;
//                return sql;
//            }
//            else return _tbAndFields;


//        }

//        static MethodInfo DataReaderGetItemMethod = typeof(IDataReader).GetMethod("get_Item", new Type[] { typeof(int) });
//        Delegate GenFill(bool retTyped = false)
//        {
//            var readerExpr = Expression.Parameter(typeof(IDataReader), "reader");
//            var indexExpr = Expression.Parameter(typeof(int), "index");
//            var valObjExpr = Expression.Parameter(typeof(object), "valObj");
//            var entityExpr = Expression.Parameter(this.Model.Type, "entity");

//            int i = 0;
//            var codes = new List<Expression>();
//            codes.Add(Expression.Assign(entityExpr, Expression.New(this.Model.Type)));
//            foreach (var member in this.Model.Members)
//            {
//                var fieldname = DbTrait.GetFieldname(member);
//                if (fieldname == null) continue;
//                //codes.Add();
//                //if(member.)
//                var readReaderExpr = Expression.Assign(
//                    valObjExpr,
//                    Expression.Call(readerExpr, DataReaderGetItemMethod, Expression.PostIncrementAssign(indexExpr))
//                );
//                codes.Add(readReaderExpr);
//                Expression convertExpr = null;
//                if (member.Nullable)
//                {
//                    var ctorMethod = member.PropertyType.GetConstructors().First(p => p.GetParameters().Length == 1);

//                    convertExpr = Expression.New(ctorMethod,
//                        Expression.Convert(valObjExpr, member.NonullableType)
//                    );
//                }
//                else
//                {
//                    convertExpr = Expression.Convert(valObjExpr, member.NonullableType);
//                }

//                var chkDbNullExpr = Expression.IfThen(
//                 Expression.NotEqual(valObjExpr, Expression.Constant(DBNull.Value))
//                 , Expression.Assign(
//                    Expression.PropertyOrField(entityExpr, member.Name)
//                    , convertExpr
//                 )
//                );
//                codes.Add(chkDbNullExpr);
//            }
//            var retLabel = Expression.Label();
//            codes.Add(Expression.Return(retLabel, retTyped ? entityExpr : (Expression)Expression.Convert(entityExpr, typeof(object))));
//            codes.Add(Expression.Label(retLabel));

//            var block = Expression.Block(new List<ParameterExpression> { valObjExpr, entityExpr }, codes);
//            //IDataReader rs;rs[]
//            var lamda = Expression.Lambda(block, readerExpr);
//            return lamda.Compile();
//        }
//    }
//}
