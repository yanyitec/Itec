namespace Itec.Metas
{
    public interface IMetaClass<T>:IMetaClass
    {
        new IMetaProperty<T> this[string name] { get; }

        TDest CopyTo<TDest>(T src, TDest dest = default(TDest), string fieldnames = null);
    }
}