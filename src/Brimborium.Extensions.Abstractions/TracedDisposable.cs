using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace Brimborium.Extensions.Abstractions {
    public class TracedDisposable : IDisposable {
        private bool _IsDisposed;
        private  readonly string _CtorStackTrace;

        public TracedDisposable() {
            if (TracedDisposableControl.Instance.IsTraceEnabled(this.GetType())) {
                this._CtorStackTrace = TracedDisposableControl.GetStackTrace();
            }
        }

        protected void SuppressFinalize() {
            System.GC.SuppressFinalize(this);
        }

        protected void ReRegisterForFinalize() {
            System.GC.ReRegisterForFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_IsDisposed) {
                if (disposing) {
                }
                _IsDisposed = true;
            }
        }

        ~TracedDisposable() {
            this.Dispose(disposing: false);
            try {
                TracedDisposableControl.Instance.ReportFinalized(
                    new ReportFinalizedInfo() { 
                        Type = this.GetType(),
                        CtorStackTrace=this._CtorStackTrace
                    });
            } catch { 
            }
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
