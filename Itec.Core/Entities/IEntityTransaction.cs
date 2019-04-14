using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Entities
{
    public interface IEntityTransaction:IDisposable
    {
        void Begin();
        Task BeginAsync();

        void Commit();

        Task CommitAsync();

        void Rollback();

        Task RollbackAsync();

        DbTransaction RawTransaction { get; }
    }
}
