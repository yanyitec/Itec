using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Itec
{
    public interface IRepository<TId,TEntity>
        where TEntity:Entity<TId>
    {

        bool Add(TEntity entity, RepoOptions opts = null);
        Task<bool> AddAsync(TEntity entity, RepoOptions opts = null);

        TEntity GetById(TId id,RepoOptions opts=null);
        Task<TEntity> GetByIdAsync(TId id, RepoOptions opts = null);

        TEntity Get(Expression<Func<TEntity,bool>> where, RepoOptions opts = null);

        Task<TEntity> GetAsync(Expression<Func<TEntity,bool>> where, RepoOptions opts = null);

        List<TEntity> List(IFilterable<TEntity> filtable, RepoOptions opts = null);
        Task<List<TEntity>> ListAsync(IFilterable<TEntity> filtable, RepoOptions opts = null);

        IPageable<TEntity> Page(IPageable<TEntity> pageable, RepoOptions opts = null);

        Task<IPageable<TEntity>> PageAsync(IPageable<TEntity> pageable, RepoOptions opts = null);



        bool Remove(TEntity entity, RepoOptions opts = null);

        Task<bool> RemoveAsync(TEntity entity, RepoOptions opts = null);

        bool Save(TEntity entity, RepoOptions opts = null);
        Task<bool> SaveAsync(TEntity entity, RepoOptions opts = null);

        long Update(TEntity data, Expression<Func<TEntity,bool>> where, RepoOptions opts = null);

        Task<long> UpdateAsync(TEntity data, Expression<Func<TEntity,bool>> where, RepoOptions opts = null);




        bool RemoveById(TId id, Expression<Func<TEntity,bool>> where = null);
        Task<bool> RemoveByIdAsync(TId id, Expression<Func<TEntity,bool>> where = null);

        long Delete( Expression<Func<TEntity,bool>> where, RepoOptions opts = null);

        Task<long> DeleteAsync(Expression<Func<TEntity, bool>> where, RepoOptions opts = null);


    }
}
