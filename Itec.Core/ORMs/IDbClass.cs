using Itec.Metas;
using System.Collections.Generic;

namespace Itec.ORMs
{
    public interface IDbClass:IMetaClass
    {
        IReadOnlyDictionary<string, IDbProperty> FieldedProps { get; }
        IDbProperty PrimaryProperty{get;}
        IDbProperty GetPrimaryProperty();

        string Tablename{get;}

        string GetTablename();
    }
}