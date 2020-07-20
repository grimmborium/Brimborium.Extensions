#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
using Xunit;
using Brimborium.Extensions.Freezable;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Extensions.Freezable {
    public class FreezableDictionaryTests {
        [Fact()]
        public void FreezableDictionary_FreezeThenAddThrows() {
            var sut = new FreezableDictionary<int, int>();
            Assert.Equal(0, sut.Count);
            sut.Add(1, 1);
            Assert.Equal(1, sut.Count);
            sut.Freeze();
            Assert.ThrowsAny<System.InvalidOperationException>(() => {
                sut.Add(2, 2);
            });
        }
    }
}