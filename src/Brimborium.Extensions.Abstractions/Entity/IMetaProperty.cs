namespace Brimborium.Extensions.Entity {
    public interface IMetaProperty {
        IMetaEntity MetaEntity { get; set; }

        string Name { get; }

        IAccessor GetAccessor(object entity);

        string ValidateType(object value, bool validateOrThrow);

        System.Type PropertyType { get; set; }
    }


}
