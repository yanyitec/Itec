using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public sealed class Nothing:IEquatable<Nothing>
    {
        public readonly static Nothing Value = new Nothing();

        public bool Equals(Nothing other)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null||obj == Nothing.Value||obj.GetType() == typeof(Nothing)) return true;
            return false;
        }
    }
}
