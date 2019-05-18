namespace Brimborium.Extensions.Destructurama.Attributed {
    using System;

    /// <summary>
    /// Specified that a property should not be included when destructuring an object for logging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotLoggedAttribute : Attribute, IPropertyDestructuringAttribute {
    }
}