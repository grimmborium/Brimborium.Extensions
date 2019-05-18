namespace Brimborium.Extensions.Entity {
    public interface IEntityFactory {
        IEntityFactory GetEntityFactory(string entityTypeName);

        IEntity CreateEntity(string entityTypeName);
    }
    public interface IEntityConcreteFactory : IEntityFactory {
        string GetEntityTypeName();
    }

    public interface IEntityDispatcherFactory : IEntityFactory {
    }
}
