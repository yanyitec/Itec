//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace CMBPS
//{
//    public class ListParameters<T> : Criteria<T>
//    {
//        public object Parameters { get; set; }

//        public ListParameters(Expression<Func<T, bool>> initCriteria = null) : base(initCriteria) {
//        }
//        [Newtonsoft.Json.JsonIgnore]
//        public long RecordCount { get; set; }

//        [Newtonsoft.Json.JsonIgnore]
//        public Expression<Func<T, object>> Asc { get; set; }
//        [Newtonsoft.Json.JsonIgnore]
//        public Expression<Func<T, object>> Desc { get; set; }
//        [Newtonsoft.Json.JsonIgnore]
//        public IEnumerable<Expression<Func<T, object>>> Includes { get; set; }

//        IList<T> _Items;

//        public IList<T> Items
//        {
//            get
//            {
//                //if (this.RecordCount == 0) return null;
//                if (_Items == null) _Items = new List<T>();
//                return _Items;
//            }
//            set
//            {
//                this._Items = value;
//                if (this.RecordCount == 0 && value != null) this.RecordCount = value.Count;
//            }
//        }

        
//    }
//}
