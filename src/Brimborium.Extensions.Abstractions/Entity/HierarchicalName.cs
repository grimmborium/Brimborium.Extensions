namespace Brimborium.Extensions.Entity {
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class HierarchicalName {
        public readonly string Name;
        public readonly HierarchicalName Parent;

        public HierarchicalName(string name) {
            this.Name = name;
        }

        public HierarchicalName(HierarchicalName parent, string name) {
            this.Parent = parent;
            this.Name = name;
        }

        public HierarchicalName Child(string name) {
            return new HierarchicalName(this, name);
        }

        private string _ToString;

        public override string ToString() {
            if ((object)this.Parent == null) {
                return this.Name;
            } else {
                return this._ToString ?? (this._ToString = $"{this.Parent.ToString()}/{this.Name}");
            }
        }

        public static implicit operator string(HierarchicalName that)
            => ((object)that == null) ? string.Empty : that.ToString();

        public static HierarchicalName operator +(HierarchicalName that, string name) {
            return new HierarchicalName(that, name);
        }
    }

}
