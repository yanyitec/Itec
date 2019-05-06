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
    public class Update<T>:Where<T>
    {
        
        public Update(SQL<T> sql):base(sql)
        {
           
            //this.Fieldnames = fieldnames;
        }

        


        

        public int Execute(Expression expr, T entity, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, entity, conn, trans);
            var effectCount= cmd.ExecuteNonQuery();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]",cmd.CommandText,effectCount);
            return effectCount;
        }

        public async Task<int> ExecuteAsync(Expression expr, T entity, DbConnection conn, DbTransaction trans)
        {
            var cmd = BuildCommand(expr, entity, conn, trans);
            var effectCount = await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]", cmd.CommandText, effectCount);
            return effectCount;
        }

        public virtual DbCommand BuildCommand(Expression expr, T entity, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            this.Sql.BuildParameters(cmd,entity);
            string sql = null;
            if (expr == null)
            {
                sql = GetUpdateSql(entity);
            }
            else {
                sql = GetUpdateSql(entity,true) + this.SqlWhere(expr,cmd);

            }
            
            cmd.CommandText = sql;
            this.Sql.Database.Logger.Debug("Executing[{0}]", cmd.CommandText);

            return cmd;
        }


        string _UpdateWithoutWhereSql;
        string _UpdateWithWhereSql;
        protected string GetUpdateSql(T data, bool withWhere = false) {
            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value) return withWhere? GenUpdateSql(data) + " WHERE ": GenUpdateSql(data);
            if (_UpdateWithoutWhereSql == null) {
                lock (this) {
                    if (_UpdateWithoutWhereSql == null)
                    {
                        _UpdateWithoutWhereSql = GenUpdateSql(data);
                        _UpdateWithWhereSql = _UpdateWithoutWhereSql + " WHERE ";
                    }
                }
            }
            return withWhere? _UpdateWithWhereSql : _UpdateWithoutWhereSql;
        }
        protected string GenUpdateSql(T data)
        {
            
            var sql = $"UPDATE {this.Sql.Tablename(true)} SET ";
            var hasField = false;
            foreach (var prop in this.Sql.AllowedProps.Values) {
                if (hasField) sql += " , "; else hasField = true;
                sql += this.Sql.DbTrait.SqlFieldname(prop.Field.Name) + "=";
                switch (this.Sql.DbTrait.ParametricKind) {
                    case SqlParametricKinds.At:
                        sql += "@" + prop.Field.Name;break;
                    case SqlParametricKinds.Question:
                        sql += "?";break;
                    default:
                        sql += SQL<T>.SqlValue(prop.GetValue(data), prop.Field.Nullable, prop.DefaultValue);
                        break;
                }
            }
            return sql;
        }

        
    }
}
