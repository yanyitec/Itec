using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Itec
{
    /// <summary>
    /// 全局同步对象
    /// </summary>
    public static class SyncObject
    {
        public readonly static object Locker = new object();
        public readonly static ReaderWriterLockSlim RWLocker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    }
}
