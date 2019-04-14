using Itec.Declaratives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Itec.ORMs
{
    public class DbField:IEquatable<DbField>
    {
        public readonly static DbField NotDbField = new DbField("<Not Field>", DbType.String, true, -1, -1, false);

        public DbField(string name, DbType dbType,bool nullable,int length,int percision,bool primary) {
            this.Name = name;
            this.DbType = dbType;
            this.Nullable = nullable;
            this.Length = length;
            this.Precision = percision;
            this.IsPrimary = primary;
        }

        public DbField(Metas.MetaProperty prop, JObject obj) {
            var fieldAttr = prop.GetAttribute<DbFieldAttribute>();
            JToken jValue = null;
            obj?.TryGetValue("FieldName", out jValue);
            if (jValue != null && jValue.Type!= JTokenType.Undefined && jValue.Type!= JTokenType.Null) {
                this.Name = jValue.ToString().Trim();
            }
            if (string.IsNullOrEmpty(this.Name)) {
                //if (prop.GetAttribute<NotDbFieldAttribute>() != null) return null;
                this.Name =(fieldAttr?.Name ?? prop.Name).Trim();  
            }
            #region type
            jValue = null;
            var propType = prop.NonullableType;
            var dbType = GetDbTypeFromType(propType);

            this.DbType = dbType;
            if (dbType == DbType.String) {
                obj?.TryGetValue("DbType", out jValue);
                if (jValue != null && jValue.Type != JTokenType.Undefined && jValue.Type != JTokenType.Null)
                {
                    System.Data.DbType dbt = DbType.Binary;
                    if (Enum.TryParse<System.Data.DbType>(jValue.ToString(), out dbt))
                    {
                        this.DbType = dbt;
                    }
                }
            }
            if (propType.IsEnum)
            {
                if (propType.GetCustomAttributes(false).Any(p => p.GetType() == typeof(FlagsAttribute)))
                {
                    this.DbType = System.Data.DbType.Int32;
                }
                else
                {
                    
                    this.DbType = System.Data.DbType.AnsiStringFixedLength;
                }
            }
            #endregion
            #region length

            if (propType.IsEnum && this.DbType != DbType.Int32)
            {
                var names = Enum.GetNames(propType);
                var max = 0;
                foreach (var n in names)
                {
                    if (n.Length > max) max = n.Length;
                }
                this.Length = ((max / 8) + 1) * 8;
            }
            else if (propType == typeof(Guid)) {
                this.Length = 64;
            }

            var lengthAttr = prop.GetAttribute<LengthAttribute>();
            if (lengthAttr != null) {
                this.Length = lengthAttr.Max;
            }

            #endregion

            #region nullable
            this.Nullable = (prop.PropertyType.IsByRef);
            if (prop.Nullable) this.Nullable = true;
            else if (prop.PropertyType == typeof(string) ) {
                if (fieldAttr != null) this.Nullable = fieldAttr.IsNullable;
                else this.Nullable = true;
            }
            #endregion

            #region Precision
            if (prop.NonullableType == typeof(Decimal)) {
                var preAttr = prop.GetAttribute<PrecisionAttribute>();
                if (preAttr != null) {
                    var len = preAttr.Integer + preAttr.Scale;
                    this.Length = len;
                    this.Precision = preAttr.Scale;
                }
            }
            #endregion

            #region primary
            if (prop.Name.ToLower() == "id") {
                this.IsPrimary = true;
                this.Nullable = false;
            }
            #endregion
        }

        public static DbType GetDbTypeFromType(Type type) {
            if (type == typeof(byte))
            {
                return System.Data.DbType.SByte;
            }

           
            if (type == typeof(short))
            {
                return System.Data.DbType.Int16;
            }

            if (type == typeof(ushort))
            {
                return System.Data.DbType.UInt16;
            }

            if (type == typeof(bool))
            {
                return System.Data.DbType.Boolean;
            }

            if (type == typeof(int)) {
                return System.Data.DbType.Int32;
            }

            if (type == typeof(uint))
            {
                return System.Data.DbType.UInt32;
            }

            if (type == typeof(long))
            {
                return System.Data.DbType.Int64;
            }

            if (type == typeof(ulong))
            {
                return System.Data.DbType.UInt64;
            }

            if (type == typeof(float))
            {
                return System.Data.DbType.Single;
            }
            if (type == typeof(double))
            {
                return System.Data.DbType.Double;
            }

            if (type == typeof(decimal))
            {
                return System.Data.DbType.Decimal;
            }

            if (type == typeof(DateTime))
            {
                return System.Data.DbType.DateTime;
            }

            if (type == typeof(Guid))
            {
                return System.Data.DbType.Guid;
            }

            return System.Data.DbType.String;
        }
        public bool IsPrimary { get; private set; }
        public string Name { get; set; }

        public DbType DbType { get;private set; }

        public bool Nullable { get; private set; }

        public int? Length { get;private set; }

        public int? Precision { get;private set; }

        public bool Equals(DbField other)
        {
            if (other == null) return false;
            return other != null
                && other.Name == this.Name
                && other.IsPrimary == this.IsPrimary
                && other.Nullable == this.Nullable
                && other.DbType == this.DbType
                && other.Length == this.Length
                && other.Precision == this.Precision;
        }
    }
}
