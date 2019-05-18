namespace Brimborium.Extensions.Destructurama.Attributed {
    using System;

    /// <summary>
    /// Specified that the type or property it is applied to should never be
    /// destructured; instead it should be logged as an atomic value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class LogAsScalarAttribute : Attribute, ITypeDestructuringAttribute, IPropertyDestructuringAttribute {
        public readonly bool IsMutable;

        /// <summary>
        /// Construct a <see cref="LogAsScalarAttribute"/>.
        /// </summary>
        /// <param name="isMutable">Whether the scalar value should be converted into a string before
        /// being passed down the (asynchronous) logging pipeline. For mutable types, specify 
        /// <code>true</code>, otherwise leave as false.</param>
        public LogAsScalarAttribute(bool isMutable = false) {
            this.IsMutable = isMutable;
        }
    }
}