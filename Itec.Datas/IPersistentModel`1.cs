using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Models
{
    public interface IPersistentModel<T> :IEnumerable<T>
        where T:class
    {

    }
}
