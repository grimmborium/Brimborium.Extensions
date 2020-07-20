using System;

namespace Brimborium.Extensions.Disposable {
    public class ContainingDisposable<T> : TracedDisposable
        where T : class, IDisposable {
        private T _Resource;

        public ContainingDisposable(T resource) : this(resource, null) { }
        public ContainingDisposable(T resource, TracedDisposableControl tracedDisposableControl) : base(tracedDisposableControl) {
            if (resource is null) {
                // no need for finalize
                this._DisposeState = (int)(DisposeState.FinalizeSuppressed);
                System.GC.SuppressFinalize(this);
            } else {
                // System.GC.ReRegisterForFinalize(this);
                this._Resource = resource;
            }
        }

        protected void SetResource(T resource) {
            if (ReferenceEquals(this._Resource, resource)) {
                return;
            }
            bool oldResource = (this._Resource is object);
            bool newResource = (resource is object);
            using (var r = this._Resource) {
                this._Resource = resource;
            }
            if (oldResource && !newResource) {
                if (InterlockedUtilty.BitwiseSet(ref this._DisposeState, (int)(DisposeState.FinalizeSuppressed))) {
                    System.GC.SuppressFinalize(this);
                }
            } else if (!oldResource && newResource) {
                if (InterlockedUtilty.BitwiseClear(ref this._DisposeState, (int)(DisposeState.FinalizeSuppressed))) {
                    System.GC.ReRegisterForFinalize(this);
                }
            }
        }

        protected T GetResource() => this._Resource;

        protected T ReadResourceAndForget() {
            var result = this._Resource;
            if (result is object) {
                this._Resource = null;
                if (InterlockedUtilty.BitwiseSet(ref this._DisposeState, (int)(DisposeState.FinalizeSuppressed))) {
                    System.GC.SuppressFinalize(this);
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing) {
            using (var r = this._Resource) {
                this._Resource = null;
            }
        }
    }    
}
