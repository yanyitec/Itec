using Itec.Entities.DB;
using Itec.Metas;
using Itec.Models;
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

namespace Itec.Entities.SQLs
{
    public class Insert
    {
        public Insert(EntityModel model, string fieldnames) {
            this.Model = model;
            this.DbSettings = model.Database.Settings;
            this.Trait = model.Database.Trait;
            //this.Fieldnames = fieldnames;
        }

        //public string Fieldnames { get; private set; }

        public EntityModel Model { get; private set; }


        public DbSettings DbSettings { get; private set; }

        public DbTrait Trait { get; private set; }

        public bool Execute(object data,DbConnection conn, DbTransaction trans) {

            var cmd = BuildCommand(data,conn,trans);
            return cmd.ExecuteNonQuery() == 1;
        }

        public async Task<bool> ExecuteAsync(object data, DbConnection conn, DbTransaction trans) {
            var cmd = BuildCommand(data, conn, trans);
            return (await cmd.ExecuteNonQueryAsync()) == 1;
        }

        public DbCommand BuildCommand(object data, DbConnection conn, DbTransaction trans) {
            var sql = GetSql(data);
            var cmd = this.Trait.CreateDbCommand(conn,sql);
            
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            _CommandBuilder(cmd);
            return cmd;
        }

        Action<DbCommand> _CommandBuilder;

        
        string _Sql;
        public string GetSql(object entity) {
            var sql = "INSERT INTO " + this.GetTableName();
            if (this.Trait.ParametricKind == SqlParametricKinds.Value)
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

        string _tbName;
        string GetTableName() {
            if (_tbName == null) {
                lock (this) {
                    if (_tbName == null) {
                        _tbName = Trait.MakeSqlTablename(this.Model,this.DbSettings.TablePrefix);
                    }
                }
            }
            
            return _tbName;
        }

        string GenSql(object entity) {
            
            
            var sql_fields = "";
            var sql_values = "";
            foreach (var prop in this.Model.Members) {
                if (prop.GetAttribute<NotFieldAttribute>() != null) continue;
                var fname = this.Trait.MakeSqlFieldname(prop);
                if (fname == null) continue;
                if (sql_fields != string.Empty) sql_fields += ",";
                if (sql_values != string.Empty) sql_values += ",";
                sql_fields += fname;
                switch (this.Trait.ParametricKind) {
                    case SqlParametricKinds.At:
                        sql_values += "@" + fname;break;
                    case SqlParametricKinds.Question:
                        sql_values += "?";break;
                    default:
                        var hasValue = prop.HasValue(entity);
                        if (!hasValue) {
                            if (prop.Nullable) sql_values += "NULL";
                            else sql_values += prop.DefaultValue.ToString();
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

        #region 
        //Action<DbCommand> GenCommandBuilder()
        //{
        //    var fields = this.Fieldnames.Split(',');
        //}
        static MethodInfo CreateParameterMethodInfo = typeof(DbCommand).GetMethod("CreateParameter");
        void GenParam(object data,MetaProperty prop,Expression cmdExpr, List<Expression> codes, List<ParameterExpression> locals)
        {
            var fname = DbTrait.GetFieldname(prop);
            var paramExpr = Expression.Parameter(typeof(DbParameter), fname);
            locals.Add(paramExpr);
            codes.Add(Expression.Assign(paramExpr,Expression.Call(cmdExpr,CreateParameterMethodInfo)));
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "ParameterName"),Expression.Constant("@" + fname)));
            DbType dbType = DbType.String;
            this.Trait.DataTypeMaps.TryGetValue(prop.NonullableType.GUID, out dbType);
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "DbType"), Expression.Constant(dbType)));
            var value = prop.GetValue(data);
            Expression valueExpr = null;
            if (prop.Nullable)
            {
                int? x;

                valueExpr = Expression.Condition(
                    Expression.PropertyOrField(Expression.Constant(value), "HasValue")
                    , Expression.PropertyOrField(Expression.Constant(value), "Value")
                    , Expression.Constant(prop.DefaultValue)
                );
            }
            else {
                valueExpr = Expression.Constant(prop.PropertyType.IsClass?value.ToString():value);
            }
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "Value"), valueExpr));

            //DbParameter par;
            //par.Value
        }

        #endregion
    }
}
