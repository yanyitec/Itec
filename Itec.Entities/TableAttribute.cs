using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}
