namespace Brimborium.Extensions.Entity {
    public interface IEntityOperation {
    }

    public interface IEntityOperationWithParameter<TIn>
        : IEntityOperation {
        //where TIn : IEntity 
    }

    public interface IEntityOperationWithOutput<TOutput>
        : IEntityOperation {
    }
}
