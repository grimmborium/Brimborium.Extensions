using System.Collections.Generic;

using Xunit;

namespace Brimborium.Extensions.Disposable {
    public class InterlockedUtiltyTest {
        [Fact]
        public void BitwiseClearTest() {
            int a = 0;
            Assert.False(InterlockedUtilty.BitwiseClear(ref a, 1));
            Assert.Equal(0, a);
            int b = 1;
            Assert.True(InterlockedUtilty.BitwiseClear(ref b, 1));
            Assert.Equal(0, b);
        }
        [Fact]
        public void BitwiseSetTest() {
            int a = 0;
            Assert.True(InterlockedUtilty.BitwiseSet(ref a, 1));
            Assert.Equal(1, a);
            int b = 1;
            Assert.False(InterlockedUtilty.BitwiseSet(ref b, 1));
            Assert.Equal(1, b);
        }
        [Fact]
        public void SetNextValueTest001() {
            List<int> l = new List<int>() { 1 };
            var o = InterlockedUtilty.SetNextValue(ref l, 2, (o, a) => { var r = o ?? new List<int>(); r.Add(a); return r; }, (v)=>Assert.Equal("Cannot", "be"));
            Assert.Same(o, l);
            Assert.Equal(new List<int>() { 1, 2 }, l);
        }
        [Fact]
        public void SetNextValueTest002() {
            List<int> l = null;
            var a=InterlockedUtilty.SetNextValue(ref l, 1, (o, a) => { var r = o ?? new List<int>(); r.Add(a); return r; });
            var b=InterlockedUtilty.SetNextValue(ref l, 2, (o, a) => { var r = o ?? new List<int>(); r.Add(a); return r; });
            Assert.NotNull(l);
            Assert.Same(l, a);
            Assert.Same(a, b);
            Assert.Equal(new List<int>() { 1, 2 }, l);

        }

    }
}