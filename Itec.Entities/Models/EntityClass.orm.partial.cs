
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public partial class EntityClass<T>
    {

        public bool Insert(T entity) {
            return true;
        }
        public T GetById(object id) {
            return default(T);
        }

        public bool Save(T entity) {
            return true;
        }

    }
}
