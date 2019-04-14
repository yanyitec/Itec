using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs.SQLs
{
    public class Create<T>
    {
        public Create(SQL<T> sql) 
        {

            this.Sql = sql;
        }

        public SQL<T> Sql { get; private set; }

        public void Execute(DbConnection conn) {
            var sql = this.CreateTableSql();
            var cmd = this.Sql.DbTrait.CreateDbCommand(conn,sql);
            this.Sql.Database.Logger.Debug(sql);
            cmd.ExecuteNonQuery();
        }

        public async Task ExecuteAsync(DbConnection conn)
        {
            var sql = this.CreateTableSql();
            var cmd = this.Sql.DbTrait.CreateDbCommand(conn, sql);
            await cmd.ExecuteNonQueryAsync();
            this.Sql.Database.Logger.Debug(sql);
        }

        public string CreateTableSql()
        {
            var sb = new StringBuilder("CREATE TABLE ");

            sb.Append(this.Sql.Tablename(true));
            sb.Append("(\n");
            
            bool hasFields = false;
            foreach (var pair in this.Sql.AllowedProps)
            {
                var prop = pair.Value;
                var fieldName = prop.Field.Name;
                var sqlFieldname = this.Sql.DbTrait.SqlFieldname(fieldName);
                if (fieldName == null) continue;
                if (hasFields) sb.Append("\t,"); else { sb.Append("\t");hasFields = true; }
                sb.Append(sqlFieldname);

                sb.Append(" ").Append(this.Sql.DbTrait.GetSqlFieldType(prop));
                var precision = this.Sql.DbTrait.GetSqlPrecision(prop);
                if (!string.IsNullOrEmpty(precision)) {
                    sb.Append("(").Append(precision).Append(")");
                }

                if (prop.Field.Nullable)
                {
                    sb.Append(" NULL");
                }
                else {
                    sb.Append(" NOT NULL");
                }

                if(prop.Field.IsPrimary) sb.Append(" PRIMARY KEY");
                sb.Append("\n");

            }
            sb.Append(")");

            return sb.ToString();

        }

        

    }
}
