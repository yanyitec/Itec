using Itec.Logs;
using Itec.Metas;
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
            _DbSets = new ConcurrentDictionary<string, IDbSet>();
            this.Logger = logger ?? Itec.Logs.Logger.Default;
        }

        public Database()
        {
            _DbSets = new ConcurrentDictionary<string, IDbSet>();
        }

        public ILogger Logger { get; private set; }
        public IMetaFactory MetaFactory { get; private set; }
        public DbSettings Settings { get; private set; }
        public DbTrait Trait { get; private set; }

        

        ConcurrentDictionary<string, IDbSet> _DbSets;
        public IDbSet<T> DbSet<T>()
            where T:class
        {
            var t = typeof(T);
            var internalSet= _DbSets.GetOrAdd(t.Name,(tx)=>new InternalDbSet<T>(this,this.MetaFactory.GetClass<T>() as IDbClass) ) as InternalDbSet<T>;
            return new QueryableSet<T>(internalSet);
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

        public bool CheckTableExists<T>() where T:class {
            var model = this.DbSet<T>();
            var sql  = this.Trait.CheckTableExistsSql(model.Sql.Tablename());
            this.Logger.Debug(sql);
            return this.ExecuteNonQuery(sql)==1;
        }

        public async Task<bool> CheckTableExistsAsync<T>() where T : class
        {
            var model = this.DbSet<T>();
            var sql = this.Trait.CheckTableExistsSql(model.Sql.Tablename());
            this.Logger.Debug(sql);
            return await this.ExecuteNonQueryAsync(sql) == 1;
        }

        public void CreateTable<T>() where T:class{
            var model = this.DbSet<T>();
            var sql = model.Sql.Create.CreateTableSql();
            this.Logger.Debug(sql);
            this.ExecuteNonQuery(sql);
        }

        public async Task CreateTableAsync<T>() where T : class
        {
            var model = this.DbSet<T>();
            var sql = model.Sql.Create.CreateTableSql();
            this.Logger.Debug(sql);
            await this.ExecuteNonQueryAsync(sql);
        }

        public void DropTable<T>() where T : class {
            var model = this.DbSet<T>();
            var tb = model.Sql.Tablename(true);
            var sql = $"DROP TABLE {tb}";
            this.Logger.Debug(sql);
            this.ExecuteNonQuery(sql);
        }

        public async Task DropTableAsync<T>() where T : class
        {
            var model = this.DbSet<T>();
            var tb = model.Sql.Tablename(true);
            var sql = $"DROP TABLE {tb}";
            this.Logger.Debug(sql);
            await this.ExecuteNonQueryAsync(sql);
        }

        public void DropTableIfExists<T>() where T : class
        {
            var model = this.DbSet<T>();

            var tb =model.Sql.Tablename(false);
            var checkSql = this.Trait.CheckTableExistsSql(tb);
            var dropSql = $"DROP TABLE {this.Trait.SqlTablename(tb)}";
            using (var conn = this.CreateConnection()) {
                conn.Open();
                using (var tran = conn.BeginTransaction()) {
                    try {
                        
                        var ckCmd = conn.CreateCommand();
                        ckCmd.CommandText = checkSql;
                        ckCmd.CommandType = System.Data.CommandType.Text;
                        ckCmd.Transaction = tran;
                        this.Logger.Debug(checkSql);
                        var tbExisted = false;
                        using (var rs = ckCmd.ExecuteReader()) {
                            if (rs.Read()) {
                                tbExisted = rs.GetInt32(0)==1;
                            }
                        }
                            
                        if (tbExisted)
                        {
                            var dropCmd = conn.CreateCommand();
                            dropCmd.CommandText = dropSql;
                            this.Logger.Debug(dropSql);
                            dropCmd.CommandType = System.Data.CommandType.Text;
                            dropCmd.Transaction = tran;
                            dropCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    } catch(Exception ex) {
                        this.Logger.Error(ex);
                        tran.Rollback();
                        throw;
                    }
                    
                }
            }
        }

        public async Task DropTableIfExistsAsync<T>() where T : class
        {
            var model = this.DbSet<T>();

            var tb = model.Sql.Tablename(false);
            var checkSql = this.Trait.CheckTableExistsSql(tb);
            var dropSql = $"DROP TABLE ${this.Trait.SqlTablename(tb)}";
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
                        this.Logger.Debug(checkSql);
                        var tbExisted = false;
                        using (var rs = await ckCmd.ExecuteReaderAsync())
                        {
                            if (await rs.ReadAsync())
                            {
                                tbExisted = rs.GetInt32(0) == 1;
                            }
                        }
                        if (tbExisted)
                        {
                            var dropCmd = conn.CreateCommand();
                            dropCmd.CommandText = dropSql;
                            dropCmd.CommandType = System.Data.CommandType.Text;
                            dropCmd.Transaction = tran;
                            this.Logger.Debug(dropSql);
                            await dropCmd.ExecuteNonQueryAsync();
                        }
                        tran.Commit();
                    }
                    catch(Exception ex)
                    {
                        this.Logger.Error(ex);
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }

        IList<DbField> QueryFields<T>() where T:class
        {
            var dbset = this.DbSet<T>();
            var sql = this.Trait.QueryFieldsSql(dbset.Sql.Tablename(false));
            var result = new List<DbField>();
            this.ExecuteQuery(sql, (reader) => {
                result.Add(this.Trait.MakeField(reader));
            });
            return result;
        }


        internal protected static IDictionary<string, Database> Databases {
            get;set;
        }


        public static Database GetByName(string name = null) {
            Database db = null;
            Databases.TryGetValue(name,out db);
            return db;
        }

        public static IDbSet<T> GetDbSet<T>(string dbName)
            where T:class
        {
            if (dbName != null)
            {
                var db = Database.GetByName(dbName);
                return db.DbSet<T>();
            }
            else {
                foreach (var pair in Databases) {
                    var m = pair.Value.DbSet<T>();
                    if (m != null) return m;
                }
                return null;
            }
        }
    }
}
