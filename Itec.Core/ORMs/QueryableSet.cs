using Itec.Datas;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Itec.ORMs
{
    public class QueryableSet<T>:Datas.Dataset<T>,IDbSet<T>,IFilterable<T>
        where T:class

    {
        public InternalDbSet<T> InternalDbSet { get; private set; }

        public Database Database => InternalDbSet.Database;

        public IDbClass<T> DbClass => InternalDbSet.DbClass as IDbClass<T>;

        public SQLs.SQL<T> Sql { get; private set; }

        public QueryableSet(InternalDbSet<T> internalDbSet,string membersString=null):base(internalDbSet.DbClass as Metas.IMetaClass<T>)
        {
            this.InternalDbSet = internalDbSet;
            this.Sql = membersString==null? internalDbSet.Sql:internalDbSet.GetSQLObject(membersString);
            this.AsyncReader = InternalReadAsync;
            this.Reader = InternalRead;
        }

        


        public IDbSet<T> MembersString(string membersString)
        {
            if (membersString == this.Sql.MembersString) return this;
            this.Sql = this.InternalDbSet.GetSQLObject(membersString);
            return this;
        }


        static List<T> InternalRead(IDataset<T> ds)
        {
            var dbset = (ds as QueryableSet<T>);
            using (var conn = dbset.Database.CreateConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                List<T> result = dbset.Sql.Select.Execute(dbset, conn, null);
                return result;

            }
        }



        static async Task<List<T>> InternalReadAsync(IDataset<T> ds)
        {
            var dbset = (ds as QueryableSet<T>);
            using (var conn = dbset.Database.CreateConnection())
            {
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();
                List<T> result = await dbset.Sql.Select.ExecuteAsync(dbset, conn, null);
                return result;

            }
        }

        public ParameterExpression QueryParameter { get; protected set; }
        protected Expression _QueryExpression;
        
        public Expression<Func<T, bool>> QueryExpression
        {
            get
            {
                if (this._QueryExpression == null) return null;
                return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(this._QueryExpression, this.QueryParameter);
            }
            set
            {
                this.QueryParameter = value.Parameters[0];
                this._QueryExpression = value.Body;
            }

        }

        public Expression<Func<T, object>> AscendingExpression { get; set; }

        public IDbSet<T> Ascending(Expression<Func<T, object>> expr) {
            this.AscendingExpression = expr;
            return this;
        }




        public Expression<Func<T, object>> DescendingExpression { get; set; }

        public IDbSet<T> Descending(Expression<Func<T, object>> expr)
        {
            this.DescendingExpression = expr;
            return this;
        }

        public int TakeCount { get; set; }

        
        public int SkipCount { get; set; }

        public IDbSet<T> Take(int size)
        {
            this.TakeCount = size;
            return this;
        }

        public IDbSet<T> Skip( int count)
        {
            this.SkipCount = count;
            return this;
        }

        public IDbSet<T> Page( int index, int size = 10)
        {
            if (index <= 0) index = 1;
            if (size <= 2) size = 2;
            this.TakeCount = size;
            this.SkipCount = (index - 1) * size;
            return this;
        }






        public IDbSet<T> AndAlso(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return this;
            if (this.QueryExpression == null)
            {
                this.QueryExpression = criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.AndAlso(this._Expression, criteria);
                this.QueryExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(this.QueryExpression.Body, FilterableExtensions.Convert<T>(this, criteria.Body, criteria.Parameters[0])), this.QueryParameter);
            }
            return this;
        }



        public IDbSet<T> OrElse( Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return this;
            if (this.QueryExpression == null)
            {
                this.QueryExpression = criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.OrElse(this._Expression, criteria);
                this.QueryExpression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(this.QueryExpression.Body, FilterableExtensions.Convert<T>(this, criteria.Body, criteria.Parameters[0])), this.QueryParameter);
            }
            return this;
        }


        IDbClass IDbSet.DbClass => InternalDbSet.DbClass;

        public IDbSet<T> Insert(T data, IDbTransaction trans = null) {
            this.InternalDbSet.Insert(data,trans);
            return this;
        }
        public async Task<IDbSet<T>> InsertAsync(T data, IDbTransaction trans = null) {
            await this.InternalDbSet.InsertAsync(data,trans);
            return this;
        }

        public IDbSet<T> Query(Expression<Func<T, bool>> criteria)
        {
            this.QueryExpression = criteria;
            return this;
        }

        public IDbSet<T> Load() {
            this.Read();
            return this;
        }

        public int Count(IDbTransaction tran = null)
        {
            if (tran == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    conn.Open();
                    return this.Sql.Count.Execute(this.QueryExpression, conn, null);
                }
            }
            else
            {
                return this.Sql.Count.Execute(this.QueryExpression, tran.DbConnection, tran.DbTransaction);

            }

        }

        public async Task<int> CountAsync(IDbTransaction tran = null)
        {
            if (tran == null)
            {
                using (var conn = this.Database.CreateConnection())
                {
                    await conn.OpenAsync();
                    return await this.Sql.Count.ExecuteAsync(this.QueryExpression, conn, null);
                }
            }
            else
            {
                return await this.Sql.Count.ExecuteAsync(this.QueryExpression, tran.DbConnection, tran.DbTransaction);

            }
        }

        public T GetById(object id, IDbTransaction tran = null) {
            var result = this.InternalDbSet.GetById(id,tran);
            this.Items = new List<T>() { result };
            return result;
        }

        public async Task<T> GetByIdAsync(object id, IDbTransaction tran = null)
        {
            var result = await this.InternalDbSet.GetByIdAsync(id, tran);
            this.Items = new List<T>() { result };
            return result;
        }
    }
}
