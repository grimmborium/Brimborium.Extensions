using Brimborium.Extensions.Abstractions;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Extensions.Disposable {
    public class ActionDispose : TracedDisposable {
        private Action _OnDispose;

        public ActionDispose(Action onDispose) {
            this._OnDispose = onDispose ?? noop;
        }

        protected override void Dispose(bool disposing) {
            var onDispose = System.Threading.Interlocked.Exchange(ref _OnDispose, noop);
            onDispose();
        }

        private static void noop() { }
    }
    public class ActionDispose<T1> : TracedDisposable {
        private Action<T1> _OnDispose;
        private T1 _Arg1;

        public ActionDispose(Action<T1> onDispose, T1 arg1) {
            this._OnDispose = onDispose;
            if (this._OnDispose is null) {
                this._Arg1 = default;
            } else { 
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
