using System;

namespace Brimborium.Extensions.Disposable {
    public sealed class DummyDisposable : IDisposable {
        public bool IsDisposed;
        private readonly Action _OnDisposed;
        private readonly Action _OnFinalized;

        public DummyDisposable(Action onDisposed, Action onFinalized)
        {
            this._OnDisposed = onDisposed;
            this._OnFinalized = onFinalized;
        }
        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    this.IsDisposed = true;
                    this._OnDisposed?.Invoke();
                }
                else
                {
                    this.IsDisposed = true;
                    this._OnFinalized?.Invoke();
                }
            }
        }

        ~DummyDisposable()
        {
            this.Dispose(disposing: false);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}