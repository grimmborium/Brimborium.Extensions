namespace Brimborium.Extensions.Destructurama.Attributed {
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class LogMaskedAttribute : Attribute, IPropertyDestructuringAttribute {
        public const string DefaultMask = "***";

        public string Text { get; set; } = DefaultMask;
        public int ShowFirst { get; set; }
        public int ShowLast { get; set; }
        public bool PreserveLength { get; set; }
    }
}
