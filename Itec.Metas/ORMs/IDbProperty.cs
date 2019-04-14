namespace Itec.ORMs
{
    public interface IDbProperty: Metas.IMetaProperty
    {
        DbField Field { get; }
    }
}