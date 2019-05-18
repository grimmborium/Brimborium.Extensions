namespace Brimborium.Extensions.Entity {
    public interface IAccessor {
        IMetaProperty MetaProperty { get; }

        object Value { get; set; }
    }
}
