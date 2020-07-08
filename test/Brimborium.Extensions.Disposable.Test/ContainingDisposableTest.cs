
using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class ContainingDisposableTest {
        [Fact]
        public void ContainingDisposableActionIsCalledOnDispose()
        {
            int cntFinalized = 0;
            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };
            var d = false;
            var f = false;
            var watchDog1 = new DummyDisposable(()=> { d = true; }, () => { f = true; });
            Assert.False(watchDog1.IsDisposed);
            for (int i = 0; i < 100; i++) {
                var sut = new ContainingDisposable<DummyDisposable>(watchDog1);
                sut.Dispose();
                sut = null;
            }
            Assert.True(watchDog1.IsDisposed);
            //
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            //
            Assert.True(d);
            Assert.False(f);
        }
        [Fact]
        public void ContainingDisposableActionIsCalledOnFinalize() {
            int cntFinalized = 0;
            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };
            var d = false;
            var f = false;
            var watchDog1 = new DummyDisposable(
                    () => { d = true; }, 
                    () => { f = true; }
                );
            Assert.False(watchDog1.IsDisposed);
            for (int i = 0; i < 100; i++) {
                var sut = new ContainingDisposable<DummyDisposable>(watchDog1);
                // NO sut.Dispose();
                sut = null;
            }
            Assert.False(watchDog1.IsDisposed);
            //
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            //
            Assert.True(d);
            Assert.False(f);
        }
    }
    public class ResourceDisposableTest {
        [Fact]
        public void ResourceDisposableTest01() {
            int cntFinalized = 0;
            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };
            
            var d1 = false;
            var f1 = false;
            var watchDog1 = new DummyDisposable(() => { d1 = true; }, () => { f1 = true; });
            
            var sut = ResourceDisposable.Create(watchDog1);
            Assert.Same(watchDog1, sut.Resource);
            sut.Dispose();

            Assert.Null(sut.Resource);
            Assert.True(d1);
            Assert.False(f1);
        }
        [Fact]
        public void ResourceDisposableTest02() {
            int cntFinalized = 0;
            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cntFinalized++; };
            var d1 = false;
            var f1 = false;
            var watchDog1 = new DummyDisposable(() => { d1 = true; }, () => { f1 = true; });

            var d2 = false;
            var f2 = false;
            var watchDog2 = new DummyDisposable(() => { d2 = true; }, () => { f2 = true; });

            var sut = ResourceDisposable.Create(watchDog1);
            Assert.Same(watchDog1, sut.Resource);
            Assert.False(watchDog2.IsDisposed);
            Assert.False(d1);
            Assert.False(f1);
            
            sut.Resource = watchDog2;
            Assert.Same(watchDog2, sut.Resource);

            Assert.Same(watchDog2, sut.ReadResourceAndForget());
            Assert.Null(sut.ReadResourceAndForget());

            Assert.False(watchDog2.IsDisposed);
            Assert.False(d2);
            Assert.False(f2);

            Assert.False(((IDisposableState)sut).IsDisposed());
            Assert.True(((IDisposableState)sut).IsFinalizeSuppressed());


            sut.Resource = watchDog2;
            Assert.Same(watchDog2, sut.Resource);

            Assert.False(((IDisposableState)sut).IsDisposed());
            Assert.False(((IDisposableState)sut).IsFinalizeSuppressed());

            sut.Dispose();
            Assert.Null(sut.Resource);
            Assert.True(((IDisposableState)sut).IsDisposed());
            Assert.True(((IDisposableState)sut).IsFinalizeSuppressed());

            Assert.True(d1);
            Assert.False(f1);
        }
    }
}