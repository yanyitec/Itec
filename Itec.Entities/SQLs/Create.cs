using Itec.Entities.DB;
using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Entities.SQLs
{
    public class Create
    {
        public Create(EntityModel model)
        {
            this.Model = model;
            this.DbSettings = model.Database.Settings;
            this.Trait = model.Database.Trait;
            
        }

        public EntityModel Model { get; private set; }


        public DbSettings DbSettings { get; private set; }

        public DbTrait Trait { get; private set; }

        public void Execute(DbConnection conn) {
            var sql = this.CreateTableSql(this.Model,this.DbSettings.TablePrefix);
            var cmd = this.Trait.CreateDbCommand(conn,sql);
            cmd.ExecuteNonQuery();
        }

        public async Task ExecuteAsync(DbConnection conn)
        {
            var sql = this.CreateTableSql(this.Model, this.DbSettings.TablePrefix);
            var cmd = this.Trait.CreateDbCommand(conn, sql);
            await cmd.ExecuteNonQueryAsync();
        }

        public string CreateTableSql(EntityModel cls, string prefix = null)
        {
            var sb = new StringBuilder("CREATE TABLE ");

            sb.Append(this.Trait.MakeSqlTablename(cls, prefix));
            sb.Append("(\n");
            bool hasId = false;
            bool hasFields = false;
            foreach (EntityProperty prop in cls)
            {
                //Name
                if (prop.GetAttribute<NotFieldAttribute>() != null) continue;
                var fieldName = DbTrait.GetFieldname(prop);
                var sqlFieldname = this.Trait.MakeSqlFieldname(fieldName);
                if (fieldName == null) continue;
                if (hasFields) sb.Append("\t,"); else { sb.Append("\t");hasFields = true; }
                sb.Append(sqlFieldname);

                sb.Append(" ").Append(this.Trait.GetSqlFieldTypeAndPrecision(prop));

                if (prop.Nullable || prop.PropertyType.IsClass)
                {
                    sb.Append(" NULL");
                }
                else {
                    sb.Append(" NOT NULL");
                }

                if (fieldName == "Id" || fieldName == "id" || fieldName == "ID" || fieldName == cls.Name + "Id" || fieldName == cls.Name + "ID")
                {
                    if (!hasId) {
                        sb.Append(" PRIMARY KEY");
                    }
                    hasId = true;
                    
                }
                sb.Append("\n");

            }
            sb.Append(")");

            return sb.ToString();

        }

        

    }
}
