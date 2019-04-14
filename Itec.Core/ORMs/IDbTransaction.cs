﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Itec.ORMs
{
    public interface IDbTransaction:IDisposable
    {
        Database Database { get; }
        DbConnection DbConnection { get; }
        DbTransaction DbTransaction { get; }
    }
}
