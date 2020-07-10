#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
using Xunit;
using Brimborium.Extensions.Freezable;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Extensions.Freezable {
    public class FreezableDictionaryTests {
        [Fact()]
        public void FreezableDictionaryTest() {
            var sut = new FreezableDictionary<int, int>();
            Assert.Equal(0, sut.Count);
            sut.Add(1, 1);
            sut.Add(2, 2);
            Assert.Equal(2, sut.Count);
            sut.Freeze();
            Assert.Throws<System.InvalidOperationException>(()=> sut.Add(3, 3));
        }
   }
}