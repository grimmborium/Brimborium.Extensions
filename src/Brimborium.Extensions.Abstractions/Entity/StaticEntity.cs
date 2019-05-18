namespace Brimborium.Extensions.Entity {
    public abstract class StaticEntity : IEntity {
        protected StaticEntity() { }

        public IMetaEntity MetaData => this.GetMetaData();
        IMetaEntity IEntity.MetaData => this.GetMetaData();

        protected abstract IMetaEntity GetMetaData();
    }
}
