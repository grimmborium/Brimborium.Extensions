using Xunit;
using Brimborium.Extensions.Freezable;

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Brimborium.Extensions.Freezable {
    public class FreezableExtensionsTests {
        public class TestFreezableObject : FreezableObject {
            public string _Text;

            public TestFreezableObject() {
            }

            public string Text {
                get {
                    return this._Text;
                }
                set {
                    this.SetStringProperty(ref this._Text, value, nameof(this.Text));
                }
            }
        }

        [Fact()]
        public void AsFrozenTest() {
            var fo = new FreezableObject();
            Assert.False(fo.IsFrozen());
            FreezableExtensions.ThrowIfFrozen(fo);
            Assert.Same(fo, FreezableExtensions.AsFrozen(fo));
            Assert.True(fo.IsFrozen());
            Assert.Throws<System.InvalidOperationException>(() => FreezableExtensions.ThrowIfFrozen(fo));
            Assert.False(fo.Freeze());
        }


        [Fact()]
        public void ThrowIfFrozenTest() {
            var fo = new FreezableObject();
            Assert.False(fo.IsFrozen());
            FreezableExtensions.ThrowIfFrozen(fo);
            Assert.Throws<System.InvalidOperationException>(() => FreezableExtensions.ThrowIfNotFrozen(fo));
            try {
                FreezableExtensions.ThrowIfNotFrozen(fo);
            } catch (System.Exception error) {
                Assert.Equal("Brimborium.Extensions.Freezable.FreezableObject is NOT frozen.", error.Message);
            }
            try {
                FreezableExtensions.ThrowIfNotFrozen(fo, "xxx");
            } catch (System.Exception error) {
                Assert.Equal("xxx is NOT frozen.", error.Message);
            }


            Assert.True(fo.Freeze());
            Assert.True(fo.IsFrozen());
            Assert.Throws<System.InvalidOperationException>(() => FreezableExtensions.ThrowIfFrozen(fo));
            FreezableExtensions.ThrowIfNotFrozen(fo);
            try {
                FreezableExtensions.ThrowIfFrozen(fo);
            } catch (System.Exception error) {
                Assert.Equal("Brimborium.Extensions.Freezable.FreezableObject is frozen.", error.Message);
            }
            try {
                FreezableExtensions.ThrowIfFrozen(fo, "xxx");
            } catch (System.Exception error) {
                Assert.Equal("xxx is frozen.", error.Message);
            }

            Assert.False(fo.Freeze());
        }

        [Fact()]
        public void SetStringPropertyTest() {
            const string TextValue1 = "aa";
            string TextValue2 = TextValue1.Substring(0, 1) + TextValue1.Substring(0, 1);
            const string TextValue3 = "bb";
            Assert.NotSame(TextValue1, TextValue2);
            {
                var sut = new TestFreezableObject();
                Assert.Null(sut.Text);
                sut.Text = TextValue1;
                Assert.Same(TextValue1, sut.Text);
                sut.Text = TextValue2;
                Assert.Same(TextValue1, sut.Text);
                sut.Text = TextValue3;
                Assert.Same(TextValue3, sut.Text);
                sut.Freeze();
                Assert.Throws<System.InvalidOperationException>(() => sut.Text = TextValue1);
                Assert.Same(TextValue3, sut.Text);
            }
            {
                var sut = new TestFreezableObject();
                Assert.Null(sut.Text);
                Assert.True(FreezableExtensions.SetStringProperty(sut, ref sut._Text, TextValue1));
                Assert.Same(TextValue1, sut.Text);
                Assert.False(FreezableExtensions.SetStringProperty(sut, ref sut._Text, TextValue1));
                Assert.Same(TextValue1, sut.Text);
                Assert.False(FreezableExtensions.SetStringProperty(sut, ref sut._Text, TextValue2));
                Assert.Same(TextValue1, sut.Text);
                Assert.True(FreezableExtensions.SetStringProperty(sut, ref sut._Text, TextValue3));
                Assert.Same(TextValue3, sut.Text);
                sut.Freeze();
                Assert.Throws<System.InvalidOperationException>(() => FreezableExtensions.SetStringProperty(sut, ref sut._Text, TextValue1));
                Assert.Same(TextValue3, sut.Text);
            }
        }
    }
}