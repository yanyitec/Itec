using System;
using System.Collections.Generic;
using System.Reflection;

namespace Itec.Metas
{
    public interface IMetaProperty
    {
        IReadOnlyList<Attribute> Attributes { get; }
        IMetaClass Class { get; }
        object DefaultValue { get; }
        MemberInfo MemberInfo { get; }
        string Name { get; }
        Type NonullableType { get; }
        bool Nullable { get; }
        Type PropertyType { get; }

        object EnsureValue(object instance);
        T GetAttribute<T>() where T : Attribute;
        object GetValue(object instance);
        bool HasValue(object instance);
        void SetValue(object instance, object value);
        ValidationResult Validate(object target, ValidateOptions opts = ValidateOptions.Undefined);
        ValidationResult ValidateValue(object value, ValidateOptions opts = ValidateOptions.Undefined);
    }
}