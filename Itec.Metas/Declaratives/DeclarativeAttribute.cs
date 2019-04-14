using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Declaratives
{
    public abstract class DeclarativeAttribute:Attribute
    {
        public virtual string Name {
            get {
                var name = this.GetType().Name;
                if (name.EndsWith("Attribute")) return name.Substring(0, name.Length - "Attribute".Length);
                else return name;
            }
        }
    }
}
