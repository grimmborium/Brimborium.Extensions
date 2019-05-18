namespace Brimborium.Extensions.Entity {
    public interface IEntityFlexible {

        object GetObjectValue(int index);

        void SetObjectValue(int index, object value);
    }
}
