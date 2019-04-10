using Itec.Metas;
using Itec.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.Entities
{
    public partial class EntityModel<T>: EntityModel
        where T:class
    {
        public EntityModel(DB.Database db):base(db,typeof(T)) {
            
        }

        public EntityModel(DB.Database db,string membersString) :base(db, typeof(T),membersString){ }



        protected override EntityModel Clone(string membersString)
        {
            return new EntityModel<T>(this.Database,membersString) {
                FieldedModels = this.FieldedModels
                //Database = this.Database
            };
        }









        #region page
        int _PageIndex;
        public int PageIndex() {
            return _PageIndex <= 0 ? 1 : _PageIndex;
        }
        public EntityModel<T> PageIndex(int pageIndex) {
            
            this._PageIndex = pageIndex;
            return this;
        }
        int _PageSize;
        public int PageSize() {
            return _PageSize<=0?0:_PageSize;
        }
        public EntityModel<T> PageSize(int pageSize)
        {
            
            this._PageSize = pageSize;
            return this;
        }

        #endregion

        #region order by
        Expression<Func<T, object>> _Desc;
        public EntityModel<T> Descending(Expression<Func<T, object>> desc) {
            this._Desc = desc;
            return this;
        }
        public Expression<Func<T, object>> Descending() {
            return _Desc;
        }

        Expression<Func<T, object>> _Ascending;
        public EntityModel<T> Ascending(Expression<Func<T, object>> asc)
        {
            this._Ascending = asc;
            return this;
        }
        public Expression<Func<T, object>> Ascending()
        {
            return _Ascending;
        }
        //ascending


        #endregion
       

        #region Filters
        
        #endregion
        #region bind
        
        #endregion
    }
}
