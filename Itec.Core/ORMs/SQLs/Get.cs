using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs.SQLs
{
    public class Get<T>:Select<T>
    {
        public IDbProperty Primary { get; private set; }
        public Get(SQL<T> sql) : base(sql) {
           
        }

        public T Execute(Expression<Func<T,bool>> exp, DbConnection conn,DbTransaction tran) {
            var cmd = this.BuildCommand(exp,conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            cmd.Transaction = tran;
            using (var reader = cmd.ExecuteReader()) {
                if (reader.Read()) return this.FillEntity(reader);
                else return default(T);
            }
        }

        public async Task<T> ExecuteAsync(Expression<Func<T, bool>> exp, DbConnection conn,DbTransaction tran)
        {
            var cmd = this.BuildCommand(exp,conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters), cmd.CommandText);
            cmd.Transaction = tran;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync()) return this.FillEntity(reader);
                else return default(T);
            }
        }

        DbCommand BuildCommand(Expression<Func<T, bool>> exp,DbConnection conn) {
            var cmd = conn.CreateCommand();
            cmd.CommandText = base.SqlWhere(exp,cmd,null);
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
                            var sqltext = this.Sql.SqlTableAndFieldsWithWhere;
                            var pk = this.Primary = this.Sql.DbClass.GetPrimaryProperty();
                            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value)
                            {
                                sqltext += $" WHERE {this.Sql.DbTrait.SqlFieldname(pk.Field.Name)}=";
                            }
                            else if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.At)
                            {
                                sqltext += $" WHERE {this.Sql.DbTrait.SqlFieldname(pk.Field.Name)}=@p0";
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
