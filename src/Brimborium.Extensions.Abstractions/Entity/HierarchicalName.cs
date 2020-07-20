namespace Brimborium.Extensions.Entity {
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class HierarchicalName {
        public readonly HierarchicalNameKind Kind;
        public readonly string Name;
        public readonly int Index;
        public readonly HierarchicalName Parent;

        public HierarchicalName(string name) {
            this.Kind = HierarchicalNameKind.Name;
            this.Name = name;
        }

        public HierarchicalName(int index) {
            this.Kind = HierarchicalNameKind.Index;
            this.Name = null;
            this.Index = index;
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
            if (this._ToString is object) {
                return this._ToString;
            } else {
                string name;
                if (this.Kind == HierarchicalNameKind.Index) {
                    name = this.Index.ToString();
                } else {
                    name = this.Name ?? string.Empty;
                }
                if (this.Parent is object) {
                    name = (this._ToString = $"{this.Parent}/{name}");
                }
                this._ToString = name;
                return name;
            }
        }

        public static implicit operator string(HierarchicalName that)
            => (that is null) ? string.Empty : that.ToString();

        public static HierarchicalName operator +(HierarchicalName that, string name) {
            return new HierarchicalName(that, name);
        }
    }
    public enum HierarchicalNameKind {
        Name,
        Index
    }
}
