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
    public class Save<T>: Where<T>
    {
        
        public Save(SQL<T> sql):base(sql)
        {
            
        }

        

        
        

        public bool Execute(T entity,Expression<Func<T,bool>> exp, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(entity,exp, conn, trans);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            var effectCount= cmd.ExecuteNonQuery();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]",cmd.CommandText,effectCount);
            return effectCount==1;
        }

        public async Task<bool> ExecuteAsync(T entity,Expression<Func<T, bool>> exp, DbConnection conn, DbTransaction trans)
        {
            var cmd = BuildCommand( entity,exp, conn, trans);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            var effectCount = await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]", cmd.CommandText, effectCount);
            return effectCount==1;
        }

        public virtual DbCommand BuildCommand(T entity, Expression<Func<T, bool>> exp, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            this.Sql.BuildParameters(cmd,entity,this.Sql.DbClass.PrimaryProperty);
            cmd.CommandText = GetSaveSql(entity,exp,cmd,this.Sql.DbClass.PrimaryProperty);
            
            return cmd;
        }


        
        string _SaveSql;
        protected string GetSaveSql(T data, Expression<Func<T, bool>> exp,DbCommand cmd, IDbProperty primaryProperty) {
            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value || exp!=null) return this.GenSaveSql(data,exp,cmd,primaryProperty);
            if (_SaveSql == null) {
                lock (this) {
                    if (_SaveSql == null)
                    {
                        _SaveSql = GenSaveSql(data,null,cmd,primaryProperty);
                    }
                }
            }
            return _SaveSql;
        }

        

        
        protected string GenSaveSql(T data, Expression<Func<T, bool>> exp,DbCommand cmd, IDbProperty idfn)
        {
            
            var sql = $"UPDATE {this.Sql.Tablename(true)} SET ";
            var hasField = false;
            //var idfn = this.GetIdField();
            foreach (var prop in this.Sql.AllowedProps.Values) {
                if (hasField) sql += " , "; else hasField = true;
                if(prop.Field.Name==idfn.Field.Name)continue;
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
            sql += " WHERE " + this.Sql.DbTrait.SqlFieldname(idfn.Field.Name)+ "=";
            switch (this.Sql.DbTrait.ParametricKind) {
                    case SqlParametricKinds.At:
                        sql += "@" + idfn;break;
                    case SqlParametricKinds.Question:
                        sql += "?";break;
                    default:
                        sql += SQL<T>.SqlValue(idfn.GetValue(data), false, idfn.DefaultValue);
                        break;
                }

            if (exp != null) {
                var where = base.SqlWhere(exp, cmd, new WhereOpts() { });
                sql += " AND " + where;
            }
            
            return sql;
        }

        
    }
}
