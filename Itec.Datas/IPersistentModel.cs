using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Models
{
    public interface IPersistentModel:IModel
    {
        IPersistentModel Load();
        IPersistentModel Store();
    }
}
