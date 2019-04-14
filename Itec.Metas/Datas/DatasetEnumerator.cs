using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public class DatasetEnumerator<T> : IEnumerator<T>
        where T : class
    {
        Dataset<T> _Dataset;
        IEnumerator<T> _ItemsEnumerator;
        public DatasetEnumerator(Dataset<T> ds) {
            this._Dataset = ds;
            _ItemsEnumerator = ds._Items?.GetEnumerator();
        }

        T _Current;
        public T Current {
            get{
                if (_ItemsEnumerator != null) return _ItemsEnumerator.Current;
                return _Current;
            }
        }

        object IEnumerator.Current {
            get {
                if (_ItemsEnumerator != null) return _ItemsEnumerator.Current;
                return _Current;
            }
        }

        public void Dispose()
        {
            if (_ItemsEnumerator != null) _ItemsEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (this._ItemsEnumerator != null) {
                var rs = this._ItemsEnumerator.MoveNext();
                if (rs == true) return true;
                this._ItemsEnumerator = null;
            }
            if (this._Dataset.IsFullfilled) return false;
            return (this._Current = this._Dataset.Read()) != null;
        }

        public async Task<bool> MoveNextAsync()
        {
            if (this._ItemsEnumerator != null)
            {
                var rs = this._ItemsEnumerator.MoveNext();
                if (rs == true) return true;
                this._ItemsEnumerator = null;
            }
            if (this._Dataset.IsFullfilled) return false;
            return (this._Current = await this._Dataset.ReadAsync()) != null;
        }

        public void Reset()
        {
            _ItemsEnumerator = this._Dataset._Items?.GetEnumerator();
        }
    }
}
