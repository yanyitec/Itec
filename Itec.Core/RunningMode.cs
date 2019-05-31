using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public static class RunMode
    {
        public enum Modes {
            Devalopment,
            Install,
            Test,
            Product

        }
        static Func<Modes> _SetMode;
        public static Func<Modes> SetMode
        {
            get
            {
                Func<Modes> sm = null;
                lock (SyncObject.Locker) {
                    sm = _SetMode;
                }
                return sm;
            }
            set {
                if (_SetMode != null) throw new InvalidOperationException("RunningMode.SetMode只能赋值一次，已经设置了SetMode函数");
                lock (SyncObject.Locker)
                {
                    _SetMode=value;
                }
            }
        }

        static Modes? _Mode;
        public static Modes Mode {
            get {
                if (_Mode == null) {
                    lock (SyncObject.Locker) {
                        if (_Mode == null) {
                            if (SetMode != null) _Mode = SetMode();
                            _Mode = DefaultSetModes();
                        }
                    }
                }
                return _Mode.Value;
            }
        }

        static Modes DefaultSetModes() {
            return Modes.Devalopment;
        }
    }
}
