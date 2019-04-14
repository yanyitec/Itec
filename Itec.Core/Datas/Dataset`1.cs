using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public class Dataset<T> : Data<T>, IDataset<T>
        where T : class
    {
        internal List<T> _Items;
        T _Current;

        protected Func<T> Reader;
        protected Func<Task<T>> AsyncReader;
        public bool IsFullfilled { get; protected set; }
        protected Dataset(Metas.IMetaFactory metaFactory = null) : base(metaFactory) { }
        protected Dataset(Metas.MetaClass<T> cls) : base(cls) { }

        public Dataset(Func<T> reader, Metas.IMetaFactory metaFactory = null) : base( metaFactory) {
            Reader = reader;

        }

        public Dataset(Func<Task<T>> reader, Metas.IMetaFactory metaFactory = null) : base(metaFactory)
        {
            AsyncReader = reader;

        }

        internal protected T Read() {
            if (this._Items == null) this._Items = new List<T>();
            T item = null;
            if (Reader != null) item = Reader();
            else {
                var task = AsyncReader();
                task.RunSynchronously();
                item = task.Result;
            }
            if (item == null) { IsFullfilled = true; return null; }
            if (this._Current == null) this._Current = item;
            
            this._Items.Add(item);
            return item;
        }

        internal protected async Task<T> ReadAsync()
        {
            if (this._Items == null) this._Items = new List<T>();
            T item = null;
            if (AsyncReader != null) item = await AsyncReader();
            else
            {
                item = await Task.Run<T>(()=>this.Read());
               
            }
            if (item == null) { IsFullfilled = true; return null; }
            if (this._Current == null) this._Current = item;

            this._Items.Add(item);
            return item;
        }
        protected T Current {
            get {
                if (_Current == null)
                {
                    return Read();
                }
                else return _Current;
            }
        }



        public int Count {
            get {
                while (!this.IsFullfilled) {
                    this.Read();
                }
                return this._Items.Count;
            }
        }

        public void Each(Action<T, int> eacher)
        {
            int index = 0;
            if (this._Items != null) {
                foreach (var item in this._Items) {
                    eacher(item,index++);
                }
            }
            while (!this.IsFullfilled) {
                eacher(this.Read(),index++);
            }
        }

        DatasetEnumerator<T> _Enumerator;

        public T Each()
        {
            if (_Enumerator == null) {
                _Enumerator = new DatasetEnumerator<T>(this);
            }
            if (_Enumerator.MoveNext()) return _Enumerator.Current;
            else _Enumerator = null;
            return null;
        }

        public void Each(Action<object, int> eacher)
        {
            int index = 0;
            if (this._Items != null)
            {
                foreach (var item in this._Items)
                {
                    eacher(item, index++);
                }
            }
            while (!this.IsFullfilled)
            {
                eacher(this.Read(), index++);
            }
        }

        public async Task<T> EachAsync()
        {
            if (_Enumerator == null)
            {
                _Enumerator = new DatasetEnumerator<T>(this);
            }
            if (await _Enumerator.MoveNextAsync()) return _Enumerator.Current;
            else _Enumerator = null;
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DatasetEnumerator<T>(this);
        }

        public IList<T> ToList()
        {
            while (!this.IsFullfilled) {
                this.Read();
            }
            return this._Items.ToList();
        }

        object IDataset.Each()
        {
            return this.Each();
        }

        async Task<object> IDataset.EachAsync()
        {
            return await this.EachAsync();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
