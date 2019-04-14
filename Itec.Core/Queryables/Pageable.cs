//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace CMBPS
//{
//    public class Pageable<T> : ListParameters<T>,IPageInfo
//    {
//        public Pageable(Expression<Func<T, bool>> initCriteria = null):base(initCriteria) {
//        }

//        public Pageable(IPageInfo pageInfo) : base()
//        {
//            if (pageInfo != null) {
//                this.PageIndex = pageInfo.PageIndex;
//                this.PageSize = pageInfo.PageSize;
//            }
//        }



//        public long PageCount { get; set; }
//        // 总条数
//        public long PageCounts { get; set; }

//        public long PageIndex { get; set; }

//        public uint PageSize { get; set; }





//        static public implicit operator Pageable<T>(Expression<Func<T,bool>> expr)
//        {
//            return new Pageable<T>(expr);
//        }


//    }
//}
