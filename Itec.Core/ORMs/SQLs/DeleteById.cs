using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs.SQLs
{
    public class DeleteById<T>:Where<T>
    {
        
        public DeleteById(SQL<T> sql) :base(sql) {
          
        }
        
        public bool Execute(object id,Expression<Func<T,bool>> criteria, DbConnection conn,DbTransaction tran) {
            var cmd = this.BuildCommand(id,criteria,conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            cmd.Transaction = tran;
            var effectCount = cmd.ExecuteNonQuery();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]",cmd.CommandText,effectCount);
            return effectCount==1;
        }

        public async Task<bool> ExecuteAsync(object id, Expression<Func<T, bool>> criteria, DbConnection conn,DbTransaction tran)
        {
            var cmd = this.BuildCommand(id,criteria, conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters), cmd.CommandText);
            cmd.Transaction = tran;
            var effectCount = await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]",cmd.CommandText,effectCount);
            return effectCount==1;
        }

        DbCommand BuildCommand(object id, Expression<Func<T, bool>> criteria, DbConnection conn) {
            var cmd = conn.CreateCommand();
            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value)
            {
                cmd.CommandText = this.SelectSql + $"'{SQL<T>.SqlValue(id, false)}'";
                if (criteria != null)
                {
                    var where = base.SqlWhere(criteria, cmd, new WhereOpts() { ValueEmbeded = true });
                    cmd.CommandText += " AND " + where;
                } 
                
            }
            
            else
            {
                cmd.CommandText = this.SelectSql;
                if (criteria != null)
                {
                    var where = base.SqlWhere(criteria, cmd, new WhereOpts() { ValueEmbeded = true });
                    cmd.CommandText += " AND " + where;
                }
                var par = cmd.CreateParameter();
                par.ParameterName = "@p_Delete_IdField_PAR0";
                par.DbType = this.Sql.DbClass.PrimaryProperty.Field.DbType;
                par.Value = id;
                cmd.Parameters.Add(par);
            }
           
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
            return cmd;


        }

        string _SelectSql;

        public string SelectSql {
            get {
                if (_SelectSql == null) {
                    lock (this) {
                        if (_SelectSql == null) {
                            var sqltext = "DELETE FROM " + this.Sql.Tablename(true) + " ";
                            var pk  = this.Sql.DbClass.GetPrimaryProperty();
                            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value)
                            {
                                sqltext += $" WHERE {this.Sql.DbTrait.SqlFieldname(pk.Field.Name)}=";
                            }
                            else if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.At)
                            {
                                sqltext += $" WHERE {this.Sql.DbTrait.SqlFieldname(pk.Field.Name)}=@p_Delete_IdField_PAR";
                            }
                            else
                            {
                                sqltext += $" WHERE {this.Sql.DbTrait.SqlFieldname(pk.Field.Name)}=?";
                            }
                            this._SelectSql = sqltext;
                        }
                    }
                }
                return _SelectSql;
            }
        }
        
    }
}
