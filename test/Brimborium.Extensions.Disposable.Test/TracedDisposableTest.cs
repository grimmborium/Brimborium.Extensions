using System;

using Xunit;

#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

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
                System.GC.Collect(2, GCCollectionMode.Forced);
                System.GC.WaitForPendingFinalizers();


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
        public void TracedDisposable003() {
            {
                var tdc = new TracedDisposableControl();
                tdc.SetTraceEnabledForAll(true);
                var sut = new TracedDisposable(tdc);
                IDisposableState sutDS = sut;
                Assert.False(sutDS.IsDisposed());
                Assert.False(sutDS.IsFinalizeSuppressed());
                Assert.NotNull(sutDS.GetReportFinalizedInfo().CtorStackTrace);
                sut.Dispose();
                Assert.True(sutDS.IsDisposed());
                Assert.True(sutDS.IsFinalizeSuppressed());
            }

            {
                var tdc = new TracedDisposableControl();
                var sut = new TracedDisposable(tdc);
                IDisposableState sutDS = sut;
                Assert.False(sutDS.IsDisposed());
                Assert.False(sutDS.IsFinalizeSuppressed());
                Assert.Null(sutDS.GetReportFinalizedInfo().CtorStackTrace);
                sut.Dispose();
                Assert.True(sutDS.IsDisposed());
                Assert.True(sutDS.IsFinalizeSuppressed());
            }
        }

        public class TracedDisposableTest004 : TracedDisposable {
            public static bool DisposingActual;
            public readonly bool DisposingExpected;

            public TracedDisposableTest004(
                bool disposingExpected,
                TracedDisposableControl tracedDisposableControl) : base(tracedDisposableControl) {
                this.DisposingExpected = disposingExpected;
            }


            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
                DisposingActual = disposing;
                Assert.Equal(this.DisposingExpected, disposing);
            }
        }

        [Fact]
        public void TracedDisposable004() {
            var tdc = new TracedDisposableControl();
            {

                for (int idx = 0; idx < 100; idx++) {
                    var sut = new TracedDisposableTest004(true, tdc);
                    Assert.Equal(true, sut.DisposingExpected);
                    sut.Dispose();
                    sut = null;
                }
                System.GC.Collect(2, GCCollectionMode.Forced);
                System.GC.WaitForPendingFinalizers();
                Assert.Equal(true, TracedDisposableTest004.DisposingActual);

            }
            {
                for (int idx = 0; idx < 100; idx++) {
                    var sut = new TracedDisposableTest004(false, tdc);
                    Assert.Equal(false, sut.DisposingExpected);
                    sut = null;
                }
                System.GC.Collect(2, GCCollectionMode.Forced);
                System.GC.WaitForPendingFinalizers();

                Assert.Equal(false, TracedDisposableTest004.DisposingActual);
            }

        }
    }
}
