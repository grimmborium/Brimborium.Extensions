namespace Brimborium.Extensions.Disposable {
    public class TracedDisposable : System.IDisposable {
        private readonly string _CtorStackTrace;
        private TracedDisposableControl _TracedDisposableControl;

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
            this.ReportFinalized();
            this.Dispose(disposing: false);
        }

        protected void ReportFinalized() {
            try {
                var tracedDisposableControl = this._TracedDisposableControl ?? TracedDisposableControl.Instance;
                tracedDisposableControl?.ReportFinalized(
                    new ReportFinalizedInfo() {
                        Type = this.GetType(),
                        CtorStackTrace = this._CtorStackTrace
                    });
            } catch {
            }
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
