using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IDataset<T>:IDataset,IEnumerable<T>
        where T : class
    {
        IList<T> ToList();
        void Each(Action<T,int> eacher);
        new T Each();
        new Task<T> EachAsync();
    }
}
