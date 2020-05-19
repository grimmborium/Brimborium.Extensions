using System;

namespace Brimborium.Extensions.Abstractions {
    public class ContainingDisposable<T> : TracedDisposable
        where T : class, IDisposable {
        private T _Resource;

        public ContainingDisposable(T resource) {
            if (resource is null) {
                // no need for finalize
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
            bool wasFinalizeRegistered;
            if (this._Resource is object) {
                using (var r = this._Resource) {
                    this._Resource = null;
                }
                wasFinalizeRegistered = true;
            } else {
                wasFinalizeRegistered = false;
            }

            if (resource is null) {
                if (wasFinalizeRegistered) {
                    System.GC.SuppressFinalize(this);
                }
            } else {
                this._Resource = resource;
                if (!wasFinalizeRegistered) {
                    System.GC.ReRegisterForFinalize(this);
                }
            }
        }

        protected T GetResource() => this._Resource;

        protected override void Dispose(bool disposing) {
            using (var r = this._Resource) {
                this._Resource = null;
            }
        }
    }
}
