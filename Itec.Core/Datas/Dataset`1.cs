using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public class Dataset<T> : Data<T>, IDataset<T>
    {
        List<T> _Items;

        internal protected List<T> Items {
            get { if (_Items == null) this.Read(); return _Items; }
            set {
                _Items = value;
                if (value != null) this._Current = _Items.FirstOrDefault();
                else this._Current = default(T);
            }
        }
        T _Current;

        protected Func<IDataset<T>,List<T>> Reader;
        protected Func<IDataset<T>,Task<List<T>>> AsyncReader;

        protected Dataset(Metas.IMetaFactory metaFactory = null) : base(metaFactory) { }
        protected Dataset(Metas.IMetaClass<T> cls) : base(cls) { }

        public Dataset(Func<IDataset<T>,List<T>> reader, Metas.IMetaFactory metaFactory = null) : base( metaFactory) {
            Reader = reader;

        }

        public Dataset(Func<IDataset<T>,Task<List<T>>> reader, Metas.IMetaFactory metaFactory = null) : base(metaFactory)
        {
            AsyncReader = reader;

        }

        internal protected void Read() {
            if (this.Reader != null) this.Items = this.Reader(this);
            else if (this.AsyncReader != null) {
                var task = Task.Run<List<T>>(async ()=>await this.AsyncReader(this));
                task.RunSynchronously();
                this.Items = task.Result;
            }
        }

        internal protected async Task ReadAsync()
        {
            if (this.AsyncReader != null) this._Items = await this.AsyncReader(this);
            else if (this.Reader != null)
            {
                this.Items = await Task.Run<List<T>>(()=> this.Reader(this));
               
            }
        }
        protected T Current {
            get {
                if (_Current == null)
                {
                    Read();
                }
                return _Current;
            }
        }

        public T this[int index] {
            get {
                return this.Items[index];
            }
        }

        public int Length {
            get {
                if (this._Items == null) this.Read();
                return this._Items.Count;
            }
        }

        public void Each(Action<T, int> eacher)
        {
            int index = 0;
            if (this._Items != null) {
                foreach (var item in this.Items) {
                    eacher(item,index++);
                }
            }
            
        }

        IEnumerator<T> _Enumerator;

        public T Each()
        {
            if (_Enumerator == null) {
                
                _Enumerator = this.Items.GetEnumerator();
            }
            if (_Enumerator.MoveNext()) return _Enumerator.Current;
            else _Enumerator = null;
            return default(T);
        }

        public void Each(Action<object, int> eacher)
        {
            int index = 0;
            if (this._Items != null)
            {
                foreach (var item in this.Items)
                {
                    eacher(item, index++);
                }
            }
            
        }

        

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        public virtual IList<T> ToList()
        {
           
            return this._Items.ToList();
        }

        object IDataset.Each()
        {
            return this.Each();
        }

       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
