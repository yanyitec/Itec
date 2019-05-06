
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Itec.Datas;
using Itec.Metas;
using Newtonsoft.Json.Linq;

namespace Itec.ORMs
{
    /// <summary>
    /// 实体集
    /// </summary>
    public class InternalDbSet<T> :IDbSet   where T:class
    {
        public Database Database { get; private set; }

        public IDbClass DbClass { get; private set; }

        ConcurrentDictionary<string, SQLs.SQL<T>> FieldedSqls { get; set; }

        public SQLs.SQL<T> Sql { get; protected set; }

        //IDbClass<T> IDbSet<T>.DbClass => this.DbClass as IDbClass<T>;

        public InternalDbSet(Database db,IDbClass dbCls) {
            this.Database = db;
            
            this.DbClass =dbCls;
            FieldedSqls = new ConcurrentDictionary<string, SQLs.SQL<T>>();
            this.Sql = new SQLs.SQL<T>(string.Empty,db,this.DbClass);
            //this.FieldedSqls.TryAdd(null,this.Sql);
            this.FieldedSqls.TryAdd("", this.Sql);
            this.FieldedSqls.TryAdd("*", this.Sql);
            
        }


        public SQLs.SQL<T> GetSQLObject(string membersString) {
            return this.FieldedSqls.GetOrAdd(membersString, (ms) => new SQLs.SQL<T>(ms, this.Database, this.DbClass));
        }










        public IDbSet<T> Insert(T data, IDbTransaction trans=null) {
            return new DbSet<T>(this).Insert(data,trans);
            
        }


        public async Task<IDbSet<T>> InsertAsync(T data, IDbTransaction trans = null) {
            var dbset = new DbSet<T>(this);
            return await dbset.InsertAsync(data, trans);

        }

        public IDbSet<T> MembersString(string membersString)
        {
            return new DbSet<T>(this,membersString);
        }

        public IDbSet<T> Query(Expression<Func<T, bool>> criteria)
        {
            var result = new DbSet<T>(this);
            result.QueryExpression = criteria;
            return result;
        }

        public IDbSet<T> Ascending(Expression<Func<T, object>> expr)
        {
            var result = new DbSet<T>(this);
            result.AscendingExpression = expr;
            return result;
        }

        public IDbSet<T> Descending(Expression<Func<T, object>> expr)
        {
            var result = new DbSet<T>(this);
            result.DescendingExpression = expr;
            return result;
        }

        public IDbSet<T> Take(int size)
        {
            var result = new DbSet<T>(this);
            result.TakeCount = size;
            return result;
        }

        public IDbSet<T> Skip( int count)
        {
            var result = new DbSet<T>(this);
            result.SkipCount = count;
            return result;
        }

        public IDbSet<T> Page( int index, int size = 10)
        {
            var result = new DbSet<T>(this);
            result.Page(index,size);
            return result;
        }






        public IDbSet<T> AndAlso(Expression<Func<T, bool>> criteria)
        {
            var result = new DbSet<T>(this);
            result.AndAlso(criteria);
            return result;
        }



        public IDbSet<T> OrElse(Expression<Func<T, bool>> criteria)
        {
            var result = new DbSet<T>(this);
            result.OrElse(criteria);
            return result;
        }

        public IDbSet<T> Load() {
            var result = new DbSet<T>(this);
            return result.Load();
        }

        public virtual int Length {
            get {
                return this.Count();
            }
        }

        

        public int Count(IDbTransaction tran=null)
        {
            if (tran == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    conn.Open();
                    return this.Sql.Count.Execute(null, conn, null);
                }
            }
            else {
                return this.Sql.Count.Execute(null, tran.DbConnection, tran.DbTransaction);

            }

        }

        public async Task<int> CountAsync(IDbTransaction tran=null)
        {
            if (tran == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await this.Sql.Count.ExecuteAsync(null, conn, null);
                }
            }
            else
            {
                return await this.Sql.Count.ExecuteAsync(null, tran.DbConnection, tran.DbTransaction);

            }
        }

        public T GetById(object id,IDbTransaction trans=null) {
            if (trans == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    conn.Open();
                    return this.Sql.GetById.Execute(id, conn,null);
                }
            }
            else {
                return this.Sql.GetById.Execute(id,trans.DbConnection,trans.DbTransaction);
            }
            
        }

        public async Task<T> GetByIdAsync(object id, IDbTransaction trans = null)
        {
            if (trans == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    
                    return await this.Sql.GetById.ExecuteAsync(id, conn, null);
                }
            }
            else
            {
                return await this.Sql.GetById.ExecuteAsync(id, trans.DbConnection, trans.DbTransaction);
            }

        }
    }
}
