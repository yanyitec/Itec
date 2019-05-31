using Itec.Logs;
using Itec.Metas;
using Itec.ORMs.SQLs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs
{
    public class Database
    {
        public Database(DbSettings settings, DbTrait trait, ILogger logger, IMetaFactory metaFactory=null) {
            this.Settings = settings;
            this.Trait = trait;
            this.MetaFactory = metaFactory ?? DbClassFactory.Default;
            _Repositories = new ConcurrentDictionary<string, IDbRepository>();
            this.Logger = logger ?? Itec.Logs.Logger.Default;
        }

        public Database()
        {
            _Repositories = new ConcurrentDictionary<string, IDbRepository>();
        }

        public ILogger Logger { get; private set; }
        public IMetaFactory MetaFactory { get; private set; }
        public DbSettings Settings { get; private set; }
        public DbTrait Trait { get; private set; }

        

        ConcurrentDictionary<string, IDbRepository> _Repositories;
        public IDbRepository<T> Repository<T>()
        {
            var t = typeof(T);
            return _Repositories.GetOrAdd(t.Name,(tx)=>new DbRepository<T>(new SQLBag<T>(this,this.MetaFactory.GetClass<T>() as IDbClass<T>)) as IDbRepository<T>) as IDbRepository<T>;
            
        }

        public DbConnection CreateConnection() {
            return this.Trait.CreateConnection(this.Settings.ConnectionString);
        }

        public int ExecuteNonQuery(string sql,DbConnection conn,DbTransaction trans=null)
        {
            var cmd = conn.CreateCommand();
            
            cmd.Transaction = trans;
            cmd.CommandText = sql;
            this.Logger.Debug(sql);
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string sql) {
            using (var conn = this.CreateConnection()) {
                conn.Open();
                this.Logger.Debug(sql);
                return ExecuteNonQuery(sql,conn);
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
                this.Logger.Debug(sql);
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
                this.Logger.Debug(sql);
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
                this.Logger.Debug(sql);
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

       

        public void CreateTable<T>() where T:class{
            var model = this.Repository<T>();
            model.CreateTable();
        }

        public async Task CreateTableAsync<T>() where T : class
        {
            await this.Repository<T>().CreateTableAsync();
        }

        public bool TableExists<T>()
        {
            return this.Repository<T>().TableExists();
        }

        public async Task<bool> TableExistsAsync<T>()
        {
            return await this.Repository<T>().TableExistsAsync();
        }

        public void DropTable<T>(){
            this.Repository<T>().DropTable();
        }

        public async Task DropTableAsync<T>() where T : class
        {
            await this.Repository<T>().DropTableAsync();
        }

        public void DropTableIfExists<T>() where T : class
        {
            this.Repository<T>().DropTableIfExists();
        }

        public async Task DropTableIfExistsAsync<T>() where T : class
        {
            await this.Repository<T>().DropTableIfExistsAsync();
        }

        public IList<DbField> QueryFields<T>() where T:class
        {
            return this.Repository<T>().QueryFields();
        }

        public async Task<IList<DbField>> QueryFieldsAsync<T>() where T : class
        {
            return await this.Repository<T>().QueryFieldsAsync();
        }


        internal protected static IDictionary<string, Database> Databases {
            get;set;
        }


        public static Database GetByName(string name = null) {
            Database db = null;
            Databases.TryGetValue(name,out db);
            return db;
        }

        public static IDbRepository<T> GetRepository<T>(string dbName)
            where T:class
        {
            if (dbName != null)
            {
                var db = Database.GetByName(dbName);
                return db.Repository<T>();
            }
            else {
                foreach (var pair in Databases) {
                    var m = pair.Value.Repository<T>();
                    if (m != null) return m;
                }
                return null;
            }
        }
    }
}
