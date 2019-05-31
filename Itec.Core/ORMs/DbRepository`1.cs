using Itec.Logs;
using Itec.ORMs.SQLs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs
{
    public class DbRepository<T> : IDbRepository, IDbRepository<T>
    {
        public DbRepository(SQLBag<T> fieldSqls)
        {
            this.SqlBag = fieldSqls;
        }
        public SQLBag<T> SqlBag { get; private set; }

        public ILogger Logger { get { return this.Database.Logger; } }

        public Database Database { get { return this.SqlBag.Database; } }

        #region Insert
        public bool Insert(T data, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans.DbTransaction;

                return sql.Insert.Execute(data, conn, dbTran) == 1;
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Insert.Execute(data, conn, null) == 1;
                }
            }
        }

        public async Task<bool> InsertAsync(T data, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans.DbTransaction;

                return await sql.Insert.ExecuteAsync(data, conn, dbTran) == 1;
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Insert.ExecuteAsync(data, conn, null) == 1;
                }
            }
        }
        #endregion

        #region List
        public List<T> List(IFilterable<T> filtable, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.Select.Execute(filtable, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Select.Execute(filtable, conn, null);
                }
            }
        }

        public async Task<List<T>> ListAsync(IFilterable<T> filtable, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans.DbTransaction;

                return await sql.Select.ExecuteAsync(filtable, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Select.ExecuteAsync(filtable, conn, null);
                }
            }
        }
        #endregion

        #region Page
        public IPageable<T> Page(IPageable<T> pageable, RepoOptions opts = null)
        {
            var pageInfo = pageable as Pageable<T>;
            if (pageInfo == null) pageInfo = new Pageable<T>(pageable);

            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans.DbTransaction;
                pageInfo.RecordCount = sql.Count.Execute(pageInfo.QueryExpression, conn, dbTran);


                pageInfo.Items = sql.Select.Execute(pageInfo, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    pageInfo.RecordCount = sql.Count.Execute(pageInfo.QueryExpression, conn, null);


                    pageInfo.Items = sql.Select.Execute(pageInfo, conn, null);
                }

            }

            return pageInfo;

        }

        public async Task<IPageable<T>> PageAsync(IPageable<T> pageable, RepoOptions opts = null)
        {
            var pageInfo = pageable as Pageable<T>;
            if (pageInfo == null) pageInfo = new Pageable<T>(pageable);

            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans.DbTransaction;
                pageInfo.RecordCount = await sql.Count.ExecuteAsync(pageInfo.QueryExpression, conn, dbTran);


                pageInfo.Items = sql.Select.Execute(pageInfo, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    pageInfo.RecordCount = await sql.Count.ExecuteAsync(pageInfo.QueryExpression, conn, null);


                    pageInfo.Items = await sql.Select.ExecuteAsync(pageInfo, conn, null);
                }

            }

            return pageInfo;

        }
        #endregion

        #region Get && Get By Id
        public T GetById(object id, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.GetById.Execute(id, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.GetById.Execute(id, conn, null);
                }
            }
        }

        public async Task<T> GetByIdAsync(object id, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.GetById.ExecuteAsync(id, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.GetById.ExecuteAsync(id, conn, null);
                }
            }
        }

        public T Get(Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.Get.Execute(exp, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Get.Execute(exp, conn, null);
                }
            }
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.Get.ExecuteAsync(exp, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Get.ExecuteAsync(exp, conn, null);
                }
            }
        }

        #endregion Get

        #region DeleteById && Delete
        public bool DeleteById(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.DeleteById.Execute(id, criteria, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.DeleteById.Execute(id, criteria, conn, null);
                }
            }
        }

        public async Task<bool> DeleteByIdAsync(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.DeleteById.ExecuteAsync(id, criteria, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.DeleteById.ExecuteAsync(id, criteria, conn, null);
                }
            }
        }

        public int Delete(Expression<Func<T, bool>> criteria = null, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.Delete.Execute(criteria, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Delete.Execute(criteria, conn, null);
                }
            }
        }

        public async Task<int> DeleteAsync(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.Delete.ExecuteAsync(criteria, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Delete.ExecuteAsync(criteria, conn, null);
                }
            }
        }
        #endregion

        #region Save && Update
        public bool Save(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.Save.Execute(data, exp, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Save.Execute(data, exp, conn, null);
                }
            }
        }

        public async Task<bool> SaveAsync(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.Save.ExecuteAsync(data, exp, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Save.ExecuteAsync(data, exp, conn, null);
                }
            }
        }

        public int Update(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return sql.Update.Execute(exp, data, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    conn.Open();
                    return sql.Update.Execute(exp, data, conn, null);
                }
            }
        }

        public async Task<int> UpateAsync(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null)
        {
            SQL<T> sql = null;
            IDbTransaction trans = null;
            if (opts != null)
            {
                sql = opts.AllowedFields == null ? this.SqlBag.AllFieldsSql : this.SqlBag.GetSQLObject(opts.AllowedFields);
                trans = opts.DbTrans;
            }
            else
            {
                sql = this.SqlBag.AllFieldsSql;
            }

            if (trans != null)
            {
                System.Data.Common.DbConnection conn = trans?.DbConnection;
                System.Data.Common.DbTransaction dbTran = trans?.DbTransaction;

                return await sql.Update.ExecuteAsync(exp, data, conn, dbTran);
            }
            else
            {
                using (var conn = this.SqlBag.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await sql.Update.ExecuteAsync(exp, data, conn, null);
                }
            }
        }
        #endregion


        #region CreateTable
        public void CreateTable()
        {
            var sql = this.SqlBag.Create.CreateTableSql();
            //this.Logger.Debug(sql);
            this.Database.ExecuteNonQuery(sql);
        }

        public async Task CreateTableAsync()
        {

            var sql = this.SqlBag.Create.CreateTableSql();
            //this.Logger.Debug(sql);
            await this.Database.ExecuteNonQueryAsync(sql);
        }
        #endregion

        public bool TableExists()
        {


            var tb = this.SqlBag.Tablename(false);
            var checkSql = this.SqlBag.Database.Trait.CheckTableExistsSql(tb);
            var c = 0;
            this.Database.ExecuteQuery(checkSql, (rs) => {
                c = rs.GetInt32(0);
            });
            return c == 1;
        }

        public async Task<bool> TableExistsAsync()
        {
            var tb = this.SqlBag.Tablename(false);
            var checkSql = this.SqlBag.Database.Trait.CheckTableExistsSql(tb);
            var c = 0;
            await this.Database.ExecuteQueryAsync(checkSql,(rs)=> {
                c = rs.GetInt32(0);
            });
            return c == 1;
        }

        public void DropTable()
        {
            var tb = this.SqlBag.Tablename(true);
            var sql = $"DROP TABLE {tb}";
            this.Database.ExecuteNonQuery(sql);
        }

        public async Task DropTableAsync()
        {
            var tb = this.SqlBag.Tablename(true);
            var sql = $"DROP TABLE {tb}";
            await this.Database.ExecuteNonQueryAsync(sql);
        }

        public void DropTableIfExists()
        {

            var tb = this.SqlBag.Tablename(false);

            var checkSql = this.Database.Trait.CheckTableExistsSql(tb);
            var dropSql = $"DROP TABLE {this.Database.Trait.SqlTablename(tb)}";
            using (var conn = this.Database.CreateConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {

                        var ckCmd = conn.CreateCommand();
                        ckCmd.CommandText = checkSql;
                        ckCmd.CommandType = System.Data.CommandType.Text;
                        ckCmd.Transaction = tran;
                        this.Logger.Debug(checkSql);
                        var tbExisted = false;
                        using (var rs = ckCmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                tbExisted = rs.GetInt32(0) == 1;
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
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex);
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }

        public async Task DropTableIfExistsAsync()
        {


            var tb = this.SqlBag.Tablename(false);
            var checkSql = this.Database.Trait.CheckTableExistsSql(tb);
            var dropSql = $"DROP TABLE {this.Database.Trait.SqlTablename(tb)}";
            using (var conn = this.Database.CreateConnection())
            {
                await conn.OpenAsync();
                using (var tran = conn.BeginTransaction())
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
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex);
                        tran.Rollback();
                        throw;
                    }

                }
            }
        }

        public IList<DbField> QueryFields()
        {
            var sql = this.Database.Trait.QueryFieldsSql(this.SqlBag.Tablename(false));
            var result = new List<DbField>();
            this.Database.ExecuteQuery(sql, (reader) =>
            {
                result.Add(this.Database.Trait.MakeField(reader));
            });
            return result;
        }

        public async Task<IList<DbField>> QueryFieldsAsync()
        {
            var sql = this.Database.Trait.QueryFieldsSql(this.SqlBag.Tablename(false));
            var result = new List<DbField>();
            await this.Database.ExecuteQueryAsync(sql, (reader) =>
            {
                result.Add(this.Database.Trait.MakeField(reader));
            });
            return result;
        }
    }
}
