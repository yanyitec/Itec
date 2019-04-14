using Itec.Metas;

using Newtonsoft.Json.Linq;
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
    public class Insert<T>
    {
        public SQL<T> Sql { get; private set; }
        public Insert(SQL<T> sql)  {

            this.Sql = sql;
        }

        //public string Fieldnames { get; private set; }

        

        public bool Execute(T data,DbConnection conn, DbTransaction trans) {

            var cmd = BuildCommand(data,conn,trans);
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> ExecuteAsync(T data, DbConnection conn, DbTransaction trans) {
            var cmd = BuildCommand(data, conn, trans);
            return (await cmd.ExecuteNonQueryAsync()) == 1;
        }

        public DbCommand BuildCommand(T data, DbConnection conn, DbTransaction trans) {
            var sql = GetSql(data);
            this.Sql.Database.Logger.DebugDetails(data,sql);
            var cmd = conn.CreateCommand();
            
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            BuildParameters(cmd, data);
            return cmd;
        }

        

        
        string _Sql;
        public string GetSql(object entity) {
            var sql = "INSERT INTO " + this.Sql.Tablename(true);
            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value)
            {
                sql += GenSql(entity);
            }
            else {
                if (_Sql == null) {
                    lock (this) {
                        if (_Sql == null) {
                            sql += GenSql(entity);
                            _Sql = sql;
                        }
                    }
                }
            }
            return _Sql;
        }

        

        string GenSql(object entity) {
            
            
            var sql_fields = "";
            var sql_values = "";
            foreach (var pair in this.Sql.AllowedProps) {

                var fname = this.Sql.DbTrait.SqlFieldname(pair.Key);
                var prop = pair.Value;
                if (fname == null) continue;
                if (sql_fields != string.Empty) sql_fields += ",";
                if (sql_values != string.Empty) sql_values += ",";
                sql_fields += fname;
                switch (this.Sql.DbTrait.ParametricKind) {
                    case SqlParametricKinds.At:
                        sql_values += "@" + fname;break;
                    case SqlParametricKinds.Question:
                        sql_values += "?";break;
                    default:
                        var nullable = prop.Nullable;
                        var fieldAttr = prop.GetAttribute<DbFieldAttribute>();
                        if (fieldAttr != null && fieldAttr.IsNullable) nullable = true;

                        var hasValue = prop.HasValue(entity);
                        if (!hasValue) {
                            if (nullable) sql_values += "NULL";
                            else sql_values += DbTrait.SqlValue(prop.DefaultValue,false,prop.DefaultValue)??"''";
                        }
                        else
                        {
                            var val = prop.EnsureValue(entity);
                            if (val == null) sql_values += "''";
                            sql_values += DbTrait.SqlValue(val.ToString());
                        }
                        break;
                        
                }
            }

            return  "(" + sql_fields + ") VALUES(" +sql_values + ")";
        }

        #region Command Builder
        Action<T, DbCommand> _ParametersBuilder;

        void BuildParameters(DbCommand cmd, T data) {
            if (_ParametersBuilder == null) {
                lock (this) {
                    if (_ParametersBuilder == null) _ParametersBuilder = GenParametersBuilder();
                }
            }
            _ParametersBuilder(data,cmd);
        }
        Action<T,DbCommand> GenParametersBuilder()
        {
            ParameterExpression cmdExpr = Expression.Parameter(typeof(DbCommand), "cmd");
            ParameterExpression dataExpr = Expression.Parameter(typeof(T), "data");
            List<Expression> codes = new List<Expression>();
            List< ParameterExpression > locals = new List<ParameterExpression>();

            foreach (var pair in this.Sql.AllowedProps)
            {
                var fname = pair.Key;
                var prop = pair.Value;
                GenParam(fname, prop,dataExpr, cmdExpr, codes, locals);
            }

            var block = Expression.Block(locals, codes);
            var lamda = Expression.Lambda<Action<T, DbCommand>>(block, dataExpr, cmdExpr);
            return lamda.Compile();
        }
        static MethodInfo CreateParameterMethodInfo = typeof(DbCommand).GetMethod("CreateParameter");
        static MethodInfo AddParameterMethodInfo = typeof(DbParameterCollection).GetMethod("Add");
        void GenParam(string fname,IDbProperty prop, Expression dataExpr, Expression cmdExpr, List<Expression> codes, List<ParameterExpression> locals)
        {
            
            var paramExpr = Expression.Parameter(typeof(DbParameter), fname);
            locals.Add(paramExpr);
            codes.Add(Expression.Assign(paramExpr,Expression.Call(cmdExpr,CreateParameterMethodInfo)));
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "ParameterName"),Expression.Constant("@" + fname)));
            DbType dbType = prop.Field.DbType;
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "DbType"), Expression.Constant(dbType)));
            Expression valueExpr = Expression.PropertyOrField(dataExpr, prop.Name);



            if (prop.Field.Nullable)
            {

                if (prop.Nullable)
                {
                    valueExpr = Expression.Condition(
                        Expression.PropertyOrField(valueExpr, "HasValue")
                        , Expression.Convert(Expression.PropertyOrField(valueExpr, "Value"),typeof(object))
                        , Expression.Convert(Expression.Constant(DBNull.Value),typeof(object))
                    );
                }
                else if (prop.PropertyType == typeof(string))
                {

                    valueExpr = Expression.Condition(
                        Expression.Equal(valueExpr, Expression.Constant(null, typeof(string)))
                        , Expression.Convert(Expression.Constant(DBNull.Value), typeof(object))
                        , Expression.Convert(valueExpr,typeof(object))
                    );
                }

            }
            else
            {
                if (prop.Nullable)
                {
                    valueExpr = Expression.Condition(
                       Expression.PropertyOrField(valueExpr, "HasValue")
                       , Expression.Convert(Expression.PropertyOrField(valueExpr, "Value"), typeof(object))
                       , Expression.Convert(Expression.Constant(prop.DefaultValue), typeof(object))
                   );
                }
                else if (prop.PropertyType == typeof(string))
                {

                    valueExpr = Expression.Condition(
                        Expression.Equal(valueExpr, Expression.Constant(null, typeof(string)))
                        , Expression.Constant(string.Empty)
                        , valueExpr
                    );
                }


            }
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "Value"), Expression.Convert(valueExpr,typeof(object))));
            codes.Add(Expression.Call(Expression.Property(cmdExpr,"Parameters"),AddParameterMethodInfo,paramExpr));
            //DbParameter par;
            //par.Value
        }

        #endregion
    }
}
