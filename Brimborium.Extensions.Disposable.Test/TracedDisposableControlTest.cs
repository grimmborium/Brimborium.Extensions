using System;
using System.Collections.Generic;

using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class TracedDisposableControlTest {
        [Fact]
        public void T001_GetStackTrace() {
            /*
            TracedDisposableControlTest.T001_GetStackTrace fileName:G:\github\grimmborium\Brimborium.Extensions\test\Brimborium.Extensions.Abstractions.Test\TracedDisposableControlTest.cs@12:13
             */
            var st = TracedDisposableControl.GetStackTrace();
            Assert.StartsWith("TracedDisposableControlTest.T001_GetStackTrace fileName:", st);
            Assert.Contains(@"Brimborium.Extensions.Disposable.Test\TracedDisposableControlTest.cs@13:13", st);
        }

        [Fact]
        public void T002_SetTraceEnabledForAll() {
            var sut = new TracedDisposableControl();
            Assert.False(sut.IsTraceEnabled(null));
            Assert.False(sut.IsTraceEnabled(typeof(int)));
        }

        [Fact]
        public void T003_SetTraceEnabledForAll() {
            var sut = new TracedDisposableControl();
            sut.SetTraceEnabledForAll(true);
            Assert.True(sut.IsTraceEnabled(null));
            sut.SetTraceEnabledForAll(false);
            Assert.False(sut.IsTraceEnabled(null));
        }

        [Fact]
        public void T004_SetTraceEnabledForType_Null() {
            var sut = new TracedDisposableControl();
            sut.SetTraceEnabledForAll(false);
            Assert.False(sut.IsTraceEnabled(null));
            sut.SetTraceEnabledForAll(true);
            Assert.True(sut.IsTraceEnabled(null));
        }
        [Fact]
        public void T005_SetTraceEnabledForType() {
            var sut = new TracedDisposableControl();
            sut.SetTraceEnabledForType(typeof(IDisposable), true);
            Assert.True(sut.IsTraceEnabled(typeof(IDisposable)));
            Assert.False(sut.IsTraceEnabled(typeof(int)));

            sut.SetTraceEnabledForType(typeof(string), true);
            Assert.True(sut.IsTraceEnabled(typeof(IDisposable)));
            Assert.True(sut.IsTraceEnabled(typeof(string)));
            Assert.False(sut.IsTraceEnabled(typeof(int)));
        }

        public class TestTracedDisposable : TracedDisposable {
        }

        [Fact]
        public void T006_Dispose() {
            TracedDisposableControl.Instance.SetTraceEnabledForAll(true);
            var reported = new List<ReportFinalizedInfo>();
            TracedDisposableControl.Instance.CurrentReportFinalized = (Action<ReportFinalizedInfo>)reportFinalized;
            for (int idx = 0; idx < 100; idx++) {
                var a = new TestTracedDisposable();
                Assert.NotNull(a.ToString());
                // no Dispose !!
            }
            //System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            Assert.True(reported.Count > 90);

            void reportFinalized(ReportFinalizedInfo reportFinalizedInfo) {
                reported.Add(reportFinalizedInfo);
            }
        }

        [Fact]
        public void T007_Finalize() {
            TracedDisposableControl.Instance.SetTraceEnabledForAll(true);
            var reported = new List<ReportFinalizedInfo>();
            TracedDisposableControl.Instance.CurrentReportFinalized = (Action<ReportFinalizedInfo>)reportFinalized;
            for (int idx = 0; idx < 100; idx++) {
                var a = new TestTracedDisposable();
                Assert.NotNull(a.ToString());
            }
            System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            Assert.True(reported.Count > 0);

            void reportFinalized(ReportFinalizedInfo reportFinalizedInfo) {
                reported.Add(reportFinalizedInfo);
            }
        }
    }
}
