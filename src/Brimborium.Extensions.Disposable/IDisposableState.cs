namespace Brimborium.Extensions.Disposable {
    public interface IDisposableState {
        bool IsDisposed();

        bool IsFinalizeSuppressed();

        ReportFinalizedInfo GetReportFinalizedInfo();
    }
}
