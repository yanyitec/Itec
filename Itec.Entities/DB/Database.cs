using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Entities.DB
{
    public class Database
    {
        public Database(DbSettings settings, DbTrait trait) {
            this.Settings = settings;
            this.Trait = trait;
            _Models = new ConcurrentDictionary<string, EntityModel>();
        }

        public Database()
        {
            _Models = new ConcurrentDictionary<string, EntityModel>();
        }

        public DbSettings Settings { get; private set; }
        public DbTrait Trait { get; private set; }

        ConcurrentDictionary<string, EntityModel> _Models;
        public EntityModel<T> Model<T>()
            where T:class
        {
            var t = typeof(T);
            return _Models.GetOrAdd(t.Name,(tx)=>new EntityModel<T>(this) ) as EntityModel<T>;
        }

        public DbConnection CreateConnection() {
            return this.Trait.CreateConnection(this.Settings.ConnectionString);
        }

        public int ExecuteNonQuery(string sql) {
            using (var conn = this.CreateConnection()) {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                return cmd.ExecuteNonQuery();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            using (var conn = this.CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public int ExecuteQuery(string sql,Action<DbDataReader> readHandler)
        {
            int count = 0;
            using (var conn = this.CreateConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        readHandler(reader);
                        count++;
                    }
                }
            }
            return count;
        }

        public async Task<int> ExecuteQueryAsync(string sql, Action<DbDataReader> readHandler)
        {
            int count = 0;
            using (var conn = this.CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        readHandler(reader);
                        count++;
                    }
                }
            }
            return count;
        }

        public bool CheckTableExists<T>() where T:class {
            var model = this.Model<T>();
            var sql  = this.Trait.CheckTableExistsSql(model);
            return this.ExecuteNonQuery(sql)==1;
        }

        public async Task<bool> CheckTableExistsAsync<T>() where T : class
        {
            var model = this.Model<T>();
            var sql = this.Trait.CheckTableExistsSql(model);
            return await this.ExecuteNonQueryAsync(sql) == 1;
        }

        public void CreateTable<T>() where T:class{
            var model = this.Model<T>();
            var sql = model.Sqls.Create.CreateTableSql(model,this.Settings.TablePrefix);
            this.ExecuteNonQuery(sql);
        }

        public async Task CreateTableAsync<T>() where T : class
        {
            var model = this.Model<T>();
            var sql = model.Sqls.Create.CreateTableSql(model, this.Settings.TablePrefix);
            await this.ExecuteNonQueryAsync(sql);
        }

        public void DropTable<T>() where T : class {
            var model = this.Model<T>();
            var tb = this.Trait.MakeSqlTablename(model,this.Settings.TablePrefix);
            var sql = $"DROP TABLE ${tb}";
            this.ExecuteNonQuery(sql);
        }

        public async Task DropTableAsync<T>() where T : class
        {
            var model = this.Model<T>();
            var tb = this.Trait.MakeSqlTablename(model, this.Settings.TablePrefix);
            var sql = $"DROP TABLE ${tb}";
            await this.ExecuteNonQueryAsync(sql);
        }

        public void DropTableIfExists<T>() where T : class
        {
            var model = this.Model<T>();

            var tb = this.Trait.MakeSqlTablename(model, this.Settings.TablePrefix);
            var checkSql = this.Trait.CheckTableExistsSql(model);
            var dropSql = $"DROP TABLE ${tb}";
            using (var conn = this.CreateConnection()) {
                conn.Open();
                using (var tran = conn.BeginTransaction()) {
                    try {
                        var ckCmd = conn.CreateCommand();
                        ckCmd.CommandText = checkSql;
                        ckCmd.CommandType = System.Data.CommandType.Text;
                        ckCmd.Transaction = tran;
                        var tbExisted = ckCmd.ExecuteNonQuery() == 1;
                        if (tbExisted)
                        {
                            var dropCmd = conn.CreateCommand();
                            dropCmd.CommandText = checkSql;
                            dropCmd.CommandType = System.Data.CommandType.Text;
                            dropCmd.Transaction = tran;
                            dropCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    } catch {
                        tran.Rollback();
                        throw;
                    }
                    
                }
            }
        }

        public async Task DropTableIfExistsAsync<T>() where T : class
        {
            var model = this.Model<T>();

            var tb = this.Trait.MakeSqlTablename(model, this.Settings.TablePrefix);
            var checkSql = this.Trait.CheckTableExistsSql(model);
            var dropSql = $"DROP TABLE ${tb}";
            using (var conn = this.CreateConnection())
            {
                await conn.OpenAsync();
                using (var tran =conn.BeginTransaction())
                {
                    try
                    {
                        var ckCmd = conn.CreateCommand();
                        ckCmd.CommandText = checkSql;
                        ckCmd.CommandType = System.Data.CommandType.Text;
                        ckCmd.Transaction = tran;
                        var tbExisted = await ckCmd.ExecuteNonQueryAsync() == 1;
                        if (tbExisted)
                        {
                            var dropCmd = conn.CreateCommand();
                            dropCmd.CommandText = checkSql;
                            dropCmd.CommandType = System.Data.CommandType.Text;
                            dropCmd.Transaction = tran;
                            await dropCmd.ExecuteNonQueryAsync();
                        }
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }



        internal protected static IDictionary<string, Database> Databases {
            get;set;
        }


        public static Database GetByName(string name = null) {
            Database db = null;
            Databases.TryGetValue(name,out db);
            return db;
        }

        public static EntityModel<T> GetModel<T>(string dbName)
            where T:class
        {
            if (dbName != null)
            {
                var db = Database.GetByName(dbName);
                return db.Model<T>();
            }
            else {
                foreach (var pair in Databases) {
                    var m = pair.Value.Model<T>();
                    if (m != null) return m;
                }
                return null;
            }
        }
    }
}
