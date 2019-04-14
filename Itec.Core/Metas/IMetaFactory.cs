using System;

namespace Itec.Metas
{
    public interface IMetaFactory
    {
        MetaClass GetClass(Type type);
        MetaClass<T> GetClass<T>();
    }
}