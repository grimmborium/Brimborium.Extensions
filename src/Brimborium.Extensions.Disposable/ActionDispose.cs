using Brimborium.Extensions.Disposable;

using System;

namespace Brimborium.Extensions.Disposable {

    public class ActionDispose : TracedDisposable {
        private Action _OnDispose;

        public ActionDispose(Action onDispose) : this(onDispose, null) { }

        public ActionDispose(Action onDispose, TracedDisposableControl tracedDisposableControl) : base(tracedDisposableControl) {
            if (onDispose is object) {
                this._OnDispose = onDispose;
            } else {
                this._OnDispose = null;
                this._DisposeState = (int)(DisposeState.FinalizeSuppressed);
                System.GC.SuppressFinalize(this);
            }
        }

        protected override void Dispose(bool disposing) {
            var onDispose = System.Threading.Interlocked.Exchange(ref _OnDispose, null);
            if (onDispose is object) {
                onDispose();
            }
        }
    }

    public class ActionDispose<T1> : TracedDisposable {
        private Action<T1> _OnDispose;
        private T1 _Arg1;
        public ActionDispose(Action<T1> onDispose, T1 arg1) : this(onDispose, arg1, null) { }

        public ActionDispose(Action<T1> onDispose, T1 arg1, TracedDisposableControl tracedDisposableControl) : base(tracedDisposableControl) {
            if (onDispose is object) {
                this._OnDispose = onDispose;
                this._Arg1 = arg1;
            }
        }

        protected override void Dispose(bool disposing) {
            var onDispose = System.Threading.Interlocked.Exchange(ref _OnDispose, null);
            if (onDispose is object) {
                var arg1 = this._Arg1;
                this._Arg1 = default;
                onDispose(arg1);
            }
        }
    }

}
