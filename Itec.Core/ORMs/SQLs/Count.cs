
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
    public class Count<T> : Where<T>
    {
        public Count(SQL<T> sql) : base(sql)
        {

        }



        
        public int Execute(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, conn, trans);

            
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                else return -1;
            }
           
        }

        public async Task<int> ExecuteAsync(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = BuildCommand(expr, conn, trans);

            var result = new List<T>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
                else return -1;
            }
            
        }




        public virtual DbCommand BuildCommand(Expression expr, DbConnection conn, DbTransaction trans)
        {

            var cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = System.Data.CommandType.Text;
            var sql = GetSql(expr, cmd);
            cmd.CommandText = sql;
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters), sql);
            return cmd;
        }

        string _tbAndFields;
        string _tbAndFieldsWithWhere;

        protected string GetSql(Expression expr, DbCommand cmd)
        {
            if (_tbAndFields == null)
            {
                lock (this)
                {
                    if (_tbAndFields == null)
                    {
                        
                        _tbAndFields = $"SELECT COUNT(*) FROM {this.Sql.Tablename(true)} ";
                        _tbAndFieldsWithWhere = _tbAndFields + " WHERE ";
                    }
                }
            }
            if (expr != null)
            {
                WhereOpts wOpts = null;
                var cond = this.SqlWhere(expr, cmd, wOpts);
                if (string.IsNullOrEmpty(cond)) return _tbAndFields;
                var sql = _tbAndFieldsWithWhere + cond;
                return sql;
            }
            else return _tbAndFields;


        }
        
    }
}
