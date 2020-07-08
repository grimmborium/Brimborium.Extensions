using Xunit;
using Brimborium.Extensions.Freezable;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Extensions.Freezable {
    public class FreezableCollectionTests {
        [Fact()]
        public void FreezableCollection_FreezeThenAddThrows() {
            var sut = new FreezableCollection<int>();
            sut.Add(1);
            sut.Freeze();
            Assert.ThrowsAny<System.InvalidOperationException>(() => { 
                sut.Add(2);
            });
        }
    }
}