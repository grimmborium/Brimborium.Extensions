using System;

using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class TracedDisposableTest {
        [Fact]
        public void TracedDisposable001() {
            int cnt = 0;
            var tdc = new TracedDisposableControl();
            tdc.CurrentReportFinalized = (rfi) => { cnt++; };

            var sut = new TracedDisposable(tdc);
            Assert.False(((IDisposableState)sut).IsDisposed());
            Assert.False(((IDisposableState)sut).IsFinalizeSuppressed());
            sut.Dispose();
            Assert.True(((IDisposableState)sut).IsDisposed());
            Assert.True(((IDisposableState)sut).IsFinalizeSuppressed());

            Assert.Equal(0, cnt);
        }
            [Fact]
        public void TracedDisposable002() {

            int cnt = 0;
            var tdc = new TracedDisposableControl();
            tdc.SetTraceEnabledForAll(true);
            tdc.CurrentReportFinalized = (rfi) => { cnt++; };
            {
                for (int idx = 0; idx < 100; idx++) {
                    var sut = new TracedDisposable(tdc);
                    // NOT sut.Dispose();
                }
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();

                Assert.True(cnt > 90, $"!({cnt} > 90)");
            }
            cnt = 0;
            {
                for (int idx = 0; idx < 100; idx++) {
                    var sut = new TracedDisposable(tdc);
                    sut.Dispose();
                }

                System.GC.Collect(2, GCCollectionMode.Forced);
                System.GC.WaitForPendingFinalizers();

                Assert.Equal(0, cnt);
            }
        }

        [Fact]
        public void TracedDisposable003() { }
    }

}
