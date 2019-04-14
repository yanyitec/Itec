namespace Itec.Metas
{
    public interface IMetaProperty<T>
    {
        IMetaClass<T> Class { get; }

        object GetValue(T instance);
        void SetValue(T instance, object value);
    }
}