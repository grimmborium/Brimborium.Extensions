using System;

using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class TracedDisposableTest {
        [Fact]
        public void TracedDisposable001() {
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
        public void TracedDisposable002() { }
    }

}
