using Xunit;
using Brimborium.Extensions.Freezable;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Extensions.Freezable {
    public class FreezableObjectTests {
        [Fact()]
        public void FreezableObjectTest() {
            var sut = new FreezableObject();
            Assert.False(sut.IsFrozen());
            sut.Freeze();
            Assert.True(sut.IsFrozen());
        }
    }
}