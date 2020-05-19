namespace Brimborium.Extensions.Abstractions {
    public class TracedDisposable : System.IDisposable {
        private readonly string _CtorStackTrace;

        public TracedDisposable() {
            if (TracedDisposableControl.Instance.IsTraceEnabled(this.GetType())) {
                this._CtorStackTrace = TracedDisposableControl.GetStackTrace();
            }
        }

        protected virtual void Dispose(bool disposing) {
        }

        ~TracedDisposable() {
            this.Dispose(disposing: false);
            TracedDisposableControl.Instance?.ReportFinalized(
                new ReportFinalizedInfo() {
                    Type = this.GetType(),
                    CtorStackTrace = this._CtorStackTrace
                });

        }

        public void Dispose() {
            this.Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
