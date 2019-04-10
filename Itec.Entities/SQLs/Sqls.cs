using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities.SQLs
{
    public class Sqls
    {
        public Sqls(string membersString,EntityModel model) {
            this.Create = new Create(model);
            this.Insert = new Insert(model, membersString);
            this.Update = new Update(model, membersString);
            this.Select = new Select(model,membersString);
        }

        public Create Create { get; private set; }
        public Insert Insert { get; private set; }

        public Update Update { get; private set; }

        public Select Select { get; private set; }

        public Where Where { get; private set; }
    }
}
