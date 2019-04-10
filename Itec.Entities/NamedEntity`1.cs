using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class NamedEntity<T>:RecordEntity<T>
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
