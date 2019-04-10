using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class FieldAttribute:Attribute
    {
        public FieldAttribute(string name,bool isNullable=false) {
            this.Name = name;
            this.IsNullable = isNullable;
        }

        public string Name { get; private set; }

        //public bool IsIndex { get; private set; }

        public bool IsNullable { get; private set; }


    }
}
