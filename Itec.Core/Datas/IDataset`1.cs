using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IDataset<T>:IDataset,IEnumerable<T>
    {
        IList<T> ToList();
        void Each(Action<T,int> eacher);
        new T Each();
        T this[int index] { get; }
    }
}
