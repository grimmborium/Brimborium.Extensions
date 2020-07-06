
using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class ContainingDisposableTest {
        [Fact]
        public void ActionDisposeActionIsCalledOnDispose()
        {
            int cntFinalized = 0;

            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };

            //var sut = new ActionDispose(() => { cntDisposed++; }, tdc);
            var watchDog = new DummyDisposable(null, null);
            var sut = new ContainingDisposable<DummyDisposable>(watchDog);
            sut.Dispose();
            Assert.Equal(true, watchDog.IsDisposed);
        }
    }
}