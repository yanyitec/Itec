using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class CodedEntity<T>:NamedEntity<T>
    {
        public string Code { get; set; }
    }
}
