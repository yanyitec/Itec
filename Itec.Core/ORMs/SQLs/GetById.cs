using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs.SQLs
{
    public class GetById<T>:Select<T>
    {
        public IDbProperty Primary { get; private set; }
        public GetById(SQL<T> sql) : base(sql) {
           
        }

        public T Execute(object id, DbConnection conn,DbTransaction tran) {
            var cmd = this.BuildCommand(id,conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters),cmd.CommandText);
            cmd.Transaction = tran;
            using (var reader = cmd.ExecuteReader()) {
                if (reader.Read()) return this.FillEntity(reader);
                else return default(T);
            }
        }

        public async Task<T> ExecuteAsync(object id, DbConnection conn,DbTransaction tran)
        {
            var cmd = this.BuildCommand(id, conn);
            this.Sql.Database.Logger.DebugDetails(new ParametersLogSerializer(cmd.Parameters), cmd.CommandText);
            cmd.Transaction = tran;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync()) return this.FillEntity(reader);
                else return default(T);
            }
        }

        DbCommand BuildCommand(object id, DbConnection conn) {
            var cmd = conn.CreateCommand();
            if (this.Sql.DbTrait.ParametricKind == SqlParametricKinds.Value)
            {
                cmd.CommandText = this._SelectSql + $"'{SQL<T>.SqlValue(id,false)}'";
            }
            
            else
            {
                cmd.CommandText = this._SelectSql;
                var par = cmd.CreateParameter();
                par.ParameterName = "@p0";
                par.DbType = this.Primary.Field.DbType;
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
                            var sqltext = this.BuildTbAndFieldsSql();
                            var pk = this.Primary = this.Sql.DbClass.FieldedProps.Values.FirstOrDefault(p => p.Field.IsPrimary);
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
