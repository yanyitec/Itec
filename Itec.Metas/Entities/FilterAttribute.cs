using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class FilterAttribute:Attribute
    {
        public FilterAttribute(FilterTypes type) {
            this.FilterType = type;
        }

        public FilterTypes FilterType { get; private set; }
    }
}
