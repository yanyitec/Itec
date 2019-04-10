using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Flows.Tests
{
    public class User:IUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string ToJSON() {
            return "{\"Id\":\""+Id +"\",\"Name\":\""+"Name"+"\"}";
        }

        public bool Is(IUser<Guid> other) {
            return other.Id == this.Id;
        }
    }
}
