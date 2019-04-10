using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface abc
    {
        Itec.Noneable<T> Get<T>(string name = null);
        object GetRaw(string name);
        object Get(Type type, string name = null);
    }
}
