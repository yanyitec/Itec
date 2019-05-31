using Itec.Logs;

namespace Itec.ORMs
{
    public interface IDbRepository
    {
        Database Database { get; }
        

        ILogger Logger { get; }

        
    }
}