using Itec.Metas;
using System.Collections.Generic;

namespace Itec.ORMs
{
    public interface IDbClass<T>:IDbClass,IMetaClass<T>
    {
        
    }
}