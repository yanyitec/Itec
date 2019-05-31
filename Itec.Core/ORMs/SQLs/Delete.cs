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
    public class Delete<T>:Where<T>
    {
        
        public Delete(SQL<T> sql):base(sql)
        {
           
            //this.Fieldnames = fieldnames;
        }

        


        

        public int Execute(Expression expr,  DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, conn, trans);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            var effectCount= cmd.ExecuteNonQuery();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]",cmd.CommandText,effectCount);
            return effectCount;
        }

        public async Task<int> ExecuteAsync(Expression expr,  DbConnection conn, DbTransaction trans)
        {
            var cmd = BuildCommand(expr, conn, trans);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            var effectCount = await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug("Effect Count={1}[{0}]", cmd.CommandText, effectCount);
            return effectCount;
        }

        public virtual DbCommand BuildCommand(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            
            if(expr==null){
                cmd.CommandText = this.DeleteSql;
            }else {
                cmd.CommandText = this.DeleteWithWhere + this.SqlWhere(expr,cmd);
            }
            return cmd;
        }


        string _DeleteSql;
        string _DeleteWithWhere;

        public string DeleteSql {
            get {
                if (_DeleteSql == null) {
                    lock (this) {
                        if (_DeleteSql == null) {
                            _DeleteSql = "DELETE FROM " + this.Sql.Tablename(true);
                            _DeleteWithWhere = _DeleteSql+ " WHERE ";
                            
                        }
                    }
                }
                return _DeleteSql;
            }
        }
        public string DeleteWithWhere {
            get {
                if (_DeleteWithWhere == null) {
                    lock (this) {
                        if (_DeleteWithWhere == null) {
                            _DeleteSql = "DELETE FROM " + this.Sql.Tablename(true);
                            _DeleteWithWhere = _DeleteSql+ " WHERE ";
                            
                        }
                    }
                }
                return _DeleteWithWhere;
            }
        }
        
    }
}
