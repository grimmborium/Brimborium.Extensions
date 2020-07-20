using System;

namespace Brimborium.Extensions.Disposable {
    [Flags]
    public enum DisposeState {
        Disposed = 1,
        FinalizeSuppressed = 2,
        DisposedFaulted = 4,
        DisposedAndFaulted = 5,
    }
}
