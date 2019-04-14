using Itec.Metas;
using System.Threading.Tasks;


namespace Itec.ORMs
{
    public interface IDbSet
    {
        Database Database { get; }
        IDbClass DbClass { get; }

        //SQLs.SQL Sql { get; }

        //string Tablename { get; }

        //string SqlTablename { get; }

        
        
    }
}