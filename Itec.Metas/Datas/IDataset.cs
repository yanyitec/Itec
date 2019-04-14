using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IDataset:IData
    {
        void Each(Action<object, int> eacher);
        object Each();

        Task<object> EachAsync();
        int Count { get; }
    }
}
