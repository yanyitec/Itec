using Itec.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public partial class EntityModel<T>
    {
        public IDictionary<string, ValidationResult> ValidateEntity(T entity) {
            var results = new Dictionary<string, ValidationResult>();
            foreach (var prop in this.Members) {
                var rs = prop.Validate(entity);
                if (rs != null) results.Add(prop.Name,rs);
            }
            return results;
        }

        public IDictionary<string, ValidationResult> ValidateFilter(IReadonlyModel dp)
        {
            var results = new Dictionary<string, ValidationResult>();
            var ds = dp;
            foreach (var prop in this.Members)
            {
                object value = null;
                
                switch (prop.FilterType)
                {
                    case FilterTypes.Equal:
                        value = ds.Get(prop.PropertyType, prop.Name);
                        if (!value.IsNullOrEmpty())
                        {
                            var rs = prop.ValidateValue(value, ValidateOptions.IgnoreRequire);
                            if (rs != null) results.Add(prop.Name,rs);
                        }

                        break;
                    case FilterTypes.Like:
                        value = ds.Get(prop.PropertyType, prop.Name);
                        if (!value.IsNullOrEmpty())
                        {
                            var rs = prop.ValidateValue(value, ValidateOptions.IgnoreRequire);
                            if (rs != null) results.Add(prop.Name, rs);
                        }



                        break;
                    case FilterTypes.Range:
                        var minKey = prop.Name + "_MIN";
                        bool hasRange = false;
                        value = ds.Get(prop.PropertyType, minKey);
                        if (!value.IsNullOrEmpty())
                        {
                            var rs = prop.ValidateValue(value, ValidateOptions.IgnoreRequire);
                            if (rs != null) results.Add(minKey, rs);
                            hasRange = true;
                        }
                        var maxKey = prop.Name + "_MAX";
                        value = ds.Get(prop.PropertyType, maxKey);
                        if (!value.IsNullOrEmpty())
                        {
                            var rs = prop.ValidateValue(value, ValidateOptions.IgnoreRequire);
                            if (rs != null) results.Add(maxKey, rs);
                            hasRange = true;
                        }
                        if (!hasRange)
                        {
                            value = ds.Get(prop.PropertyType, prop.Name);
                            if (!value.IsNullOrEmpty())
                            {
                                var rs = prop.ValidateValue(value, ValidateOptions.IgnoreRequire);
                                if (rs != null) results.Add(minKey, rs);
                            }

                        }

                        break;

                }
            }

            
            return results;
        }
    }
    
}
