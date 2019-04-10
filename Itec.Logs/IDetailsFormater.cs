using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Logs
{
    public interface IDetailsFormater
    {
        string Format(object details);
    }
}
