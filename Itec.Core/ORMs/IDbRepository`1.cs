using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Itec.Logs;
using Itec.ORMs.SQLs;

namespace Itec.ORMs
{
    public interface IDbRepository<T>:IDbRepository
    {
        
        SQLBag<T> SqlBag { get; }

        void CreateTable();
        Task CreateTableAsync();
        int Delete(Expression<Func<T, bool>> criteria = null, RepoOptions opts = null);
        Task<int> DeleteAsync(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null);
        bool DeleteById(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null);
        Task<bool> DeleteByIdAsync(object id, Expression<Func<T, bool>> criteria = null, RepoOptions opts = null);
        void DropTable();
        Task DropTableAsync();
        void DropTableIfExists();
        Task DropTableIfExistsAsync();
        T Get(Expression<Func<T, bool>> exp, RepoOptions opts = null);
        Task<T> GetAsync(Expression<Func<T, bool>> exp, RepoOptions opts = null);
        T GetById(object id, RepoOptions opts = null);
        Task<T> GetByIdAsync(object id, RepoOptions opts = null);
        bool Insert(T data, RepoOptions opts = null);
        Task<bool> InsertAsync(T data, RepoOptions opts = null);
        List<T> List(IFilterable<T> filtable, RepoOptions opts = null);
        Task<List<T>> ListAsync(IFilterable<T> filtable, RepoOptions opts = null);
        IPageable<T> Page(IPageable<T> pageable, RepoOptions opts = null);
        Task<IPageable<T>> PageAsync(IPageable<T> pageable, RepoOptions opts = null);
        IList<DbField> QueryFields();
        Task<IList<DbField>> QueryFieldsAsync();
        bool Save(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null);
        Task<bool> SaveAsync(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null);
        bool TableExists();
        Task<bool> TableExistsAsync();
        Task<int> UpateAsync(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null);
        int Update(T data, Expression<Func<T, bool>> exp, RepoOptions opts = null);
    }
}