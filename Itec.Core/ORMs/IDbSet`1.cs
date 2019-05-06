using Itec.Metas;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace Itec.ORMs
{
    public interface IDbSet<T>:IFilterable<T>,IDbSet
    {
        
        new IDbClass<T> DbClass { get; }

        SQLs.SQL<T> Sql { get; }

        IDbSet<T> MembersString(string membersString);

        IDbSet<T> Insert(T data,IDbTransaction trans=null);
        Task<IDbSet<T>> InsertAsync(T data, IDbTransaction trans = null);

        IDbSet<T> Query(Expression<Func<T, bool>> criteria);

        IDbSet<T> AndAlso(Expression<Func<T, bool>> criteria);

        IDbSet<T> OrElse(Expression<Func<T, bool>> criteria);

        IDbSet<T> Ascending(Expression<Func<T, object>> expr);

        IDbSet<T> Descending(Expression<Func<T, object>> expr);

        IDbSet<T> Take(int size);

        IDbSet<T> Skip(int count);

        IDbSet<T> Page(int index, int size);

        


        IDbSet<T> Load();

        T GetById(object id,IDbTransaction trans=null);
        Task<T> GetByIdAsync(object id, IDbTransaction trans = null);
    }
}