namespace Itec.ORMs
{
    public interface IDbProperty<T>: Metas.IMetaProperty<T>
    {
        DbField Field { get; }
    }
}