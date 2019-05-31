using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface IData<T>:IData
    {
        T ToObject();
    }
}
