using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORMs
{
    public class DbFieldAttribute:Attribute
    {
        public DbFieldAttribute(string name,bool isNullable=false) {
            this.Name = name;
            this.IsNullable = isNullable;
        }

        public string Name { get; private set; }

        //public bool IsIndex { get; private set; }

        public bool IsNullable { get; private set; }


    }
}
