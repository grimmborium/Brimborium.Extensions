using System;

using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class ActionDisposeTest {
        [Fact]
        public void ActionDisposeActionIsCalledOnDispose() {
            int cntDisposed = 0;
            int cntFinalized = 0;

            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };

            var sut = new ActionDispose(() => { cntDisposed++; }, tdc);
            sut.Dispose();
            Assert.Equal(1, cntDisposed);
            Assert.Equal(0, cntFinalized);
        }

        [Fact]
        public void ActionDisposeActionIsCalledOnFinalizer() {
            int cntDisposed = 0;
            int cntFinalized = 0;

            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };

            for (int idx = 0; idx < 100; idx++) {
                var sut = new ActionDispose(() => { cntDisposed++; }, tdc);
                // NO sut.Dispose();
                sut = null;
            }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            Assert.True(cntDisposed == cntFinalized, $"{cntDisposed} == {cntFinalized}");
            Assert.True(cntDisposed > 90, $"!({cntDisposed}>90)");
            Assert.True(cntFinalized > 90, $"!({cntFinalized}>90)");
        }


        [Fact]
        public void ActionDisposeNoAction() {
            int cntDisposed = 0;
            int cntFinalized = 0;

            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };

            var sut = new ActionDispose(null, tdc);
            sut.Dispose(); // no error
            Assert.Equal(0, cntDisposed);
            Assert.Equal(0, cntFinalized);
        }
        [Fact]
        public void ActionDisposeActionWithException() {
            int cntDisposed = 0;
            int cntFinalized = 0;

            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);

            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; }; 
            var sut = new ActionDispose(() => { cntDisposed++; throw new ArgumentException("HUGO"); });
            try {
                Assert.Throws<ArgumentException>(() => sut.Dispose());
            } catch {
            }
            sut.Dispose(); // no error again
            Assert.Equal(1, cntDisposed);
        }

    }

}
