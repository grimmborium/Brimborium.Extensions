namespace Brimborium.Extensions.Entity {
    using Brimborium.Extensions.Freezable;
    using System.Collections.Generic;

    public interface IMetaEntity : IFreezable {
        string EntityTypeName { get; set; }

        IMetaProperty GetProperty(string name);

        IList<IMetaProperty> GetProperties();

        string Validate(IMetaProperty metaProperty, object value, bool validateOrThrow);
    }

    public interface IMetaEntityFlexible : IMetaEntity {
        IMetaIndexedProperty GetPropertyByIndex(int index);

        IList<IMetaIndexedProperty> GetPropertiesByIndex();

        string Validate(object[] values, bool validateOrThrow);
    }
}