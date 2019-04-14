using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORMs
{
    public class DbTableAttribute : Attribute
    {
        public DbTableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}
