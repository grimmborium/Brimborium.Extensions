using System;

namespace Brimborium.Extensions.Disposable {
    public class TracedDisposable : System.IDisposable, IDisposableState {
        private readonly string _CtorStackTrace;
        private TracedDisposableControl _TracedDisposableControl;
        protected int _DisposeState;

        public TracedDisposable() : this(null) { }

        public TracedDisposable(TracedDisposableControl tracedDisposableControl) {
            this._TracedDisposableControl = tracedDisposableControl;
            tracedDisposableControl ??= TracedDisposableControl.Instance;
            if (tracedDisposableControl.IsTraceEnabled(this.GetType())) {
                this._CtorStackTrace = TracedDisposableControl.GetStackTrace();
            }
        }

        protected virtual void Dispose(bool disposing) {
        }

        ~TracedDisposable() {
            TracedDisposableControl.ReportFinalized(
                    this._TracedDisposableControl,
                    new ReportFinalizedInfo() {
                        Type = this.GetType(),
                        CtorStackTrace = this._CtorStackTrace
                    }
                );
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            try {
                this.Dispose(disposing: true);
                InterlockedUtilty.BitwiseSet(ref this._DisposeState, (int)(DisposeState.Disposed));
            } catch {
                InterlockedUtilty.BitwiseSet(ref this._DisposeState, (int)(DisposeState.Disposed | DisposeState.DisposedFaulted));
                throw;
            } finally {
                if (InterlockedUtilty.BitwiseSet(ref this._DisposeState, (int)(DisposeState.FinalizeSuppressed))) {
                    System.GC.SuppressFinalize(this);
                }
            }
        }

        bool IDisposableState.IsDisposed() => (this._DisposeState & (int)(DisposeState.Disposed)) != 0;

        bool IDisposableState.IsFinalizeSuppressed() => (this._DisposeState & (int)(DisposeState.FinalizeSuppressed)) != 0;

        ReportFinalizedInfo IDisposableState.GetReportFinalizedInfo()
            => new ReportFinalizedInfo() {
                Type = this.GetType(),
                CtorStackTrace = this._CtorStackTrace
            };
    }
    [Flags]
    public enum DisposeState {
        Disposed = 1,
        FinalizeSuppressed = 2,
        DisposedFaulted = 4,
    }
}
