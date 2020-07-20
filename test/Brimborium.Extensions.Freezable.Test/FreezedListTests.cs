#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
using Xunit;

namespace Brimborium.Extensions.Freezable {
    public class FreezedListTests {
        [Fact()]
        public void FreezedList_001() {
            var sut = new FreezedList<int>(null, false);
            Assert.Equal(0, sut.Count);
        }
        [Fact()]
        public void FreezedList_002() {
            var collection = new FreezableCollection<int>();
            collection.Add(1);
            collection.Add(2);
            var sut = FreezedList.AsFreezedList(collection);
            Assert.Equal(2, sut.Count);
            Assert.ThrowsAny<System.NotSupportedException>(() => {
                sut.Add(1);
            });
        }
        [Fact()]
        public void FreezedList_003() {
            var collection = new FreezableCollection<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Freeze();
            var sut = FreezedList.AsFreezedList(collection);
            Assert.Equal(2, sut.Count);
            Assert.ThrowsAny<System.NotSupportedException>(() => {
                sut.Add(1);
            });
        }
        [Fact()]
        public void FreezedList_004() {
            var src = new int[] { 1, 2, 3 };
            var a = FreezedList.AsFreezedList(src, true);
            var b = FreezedList.AsFreezedList(src, false);
            Assert.Equal(3, a.Count);
            Assert.Equal(3, b.Count);

            Assert.Equal(2, a[1]);
            Assert.Equal(2, b[1]);

            src[1] = 42;

            Assert.Equal(42, a[1]);
            Assert.Equal(2, b[1]);

        }
    }
}
