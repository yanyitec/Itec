using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IUser<T>
    {
        T Id { get; }
        string Name { get; }

        bool Is(IUser<T> other);
        string ToJSON();
    }
}
