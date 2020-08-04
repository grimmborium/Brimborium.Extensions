namespace Brimborium.Extensions.Decoration {
    public readonly struct ValueDecorated<TResult> {
        public readonly TResult Result { get; }
        public readonly ValueDecoration Decoration { get; }
        public ValueDecorated(TResult result, ValueDecoration decoration) {
            this.Result = result;
            this.Decoration = decoration;
        }
    }
    public class ValueDecoration {
        public ValueDecoration? Next { get; private set; }
        public ValueDecoration(ValueDecoration? prev) {
            if (prev is object) {
                prev.Next = this;
            }
        }
    }
}
