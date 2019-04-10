using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class EntityOptions
    {
        public string MembersString { get; set; }

        public IEntityTransaction Transaction { get; set; }
    }
}
