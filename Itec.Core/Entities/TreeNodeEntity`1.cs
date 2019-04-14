using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class TreeNodeEntity<T>:CodedEntity<T>
    {
        public T ParentId { get; set; }

        public TreeNodeEntity<T> Parent { get; set; }
    }
}
