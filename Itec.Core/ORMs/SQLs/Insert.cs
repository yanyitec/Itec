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
            var effectCount = cmd.ExecuteNonQuery();
            this.Sql.Database.Logger.Debug("SQL effects={1}[{0}]", cmd.CommandText, effectCount);
            return effectCount==1;
        }

        public async Task<bool> ExecuteAsync(T data, DbConnection conn, DbTransaction trans) {
            var cmd = BuildCommand(data, conn, trans);
            var effectCount = await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug("SQL effects={1}[{0}]", cmd.CommandText, effectCount);
            return effectCount == 1;
        }

        public DbCommand BuildCommand(T data, DbConnection conn, DbTransaction trans) {
            var sql = GetSql(data);
            
            var cmd = conn.CreateCommand();
            
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            this.Sql.BuildParameters(cmd, data);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),"SQL executing[{0}]" ,sql);
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
                var prop = pair.Value;
                var fname = this.Sql.DbTrait.SqlFieldname(prop.Field.Name);
               
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
                            else sql_values += SQL<T>.SqlValue(prop.DefaultValue,false,prop.DefaultValue)??"''";
                        }
                        else
                        {
                            var val = prop.EnsureValue(entity);
                            if (val == null) sql_values += "''";
                            sql_values += SQL<T>.SqlValue(val.ToString());
                        }
                        break;
                        
                }
            }

            return  "(" + sql_fields + ") VALUES(" +sql_values + ")";
        }

    }
}
