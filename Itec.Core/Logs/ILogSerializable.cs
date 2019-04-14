using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    public interface ILogSerializable
    {
        string Serialize(ILogWriter writer);
    }
}
