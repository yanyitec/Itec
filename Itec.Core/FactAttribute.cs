using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    [AttributeUsage(AttributeTargets.Assembly| AttributeTargets.Class | AttributeTargets.Method)]
    public class FactAttribute:Attribute
    {
    }
}
