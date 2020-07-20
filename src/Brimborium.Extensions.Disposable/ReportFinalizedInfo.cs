using System;

namespace Brimborium.Extensions.Disposable {
    /// <summary>
    /// Parameter of <see cref="TracedDisposableControl.ReportFinalized(ReportFinalizedInfo)"/>
    /// </summary>
    public struct ReportFinalizedInfo {
        public Type Type;
        public string CtorStackTrace;
    }
}