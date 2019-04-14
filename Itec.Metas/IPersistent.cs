using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec
{
    public interface IPersistent
    {
        IPersistent Load();
        Task<IPersistent> LoadAsync();
        IPersistent Save(object trans = null);
        Task<IPersistent> SaveAsync(object trans = null);
    }
}
