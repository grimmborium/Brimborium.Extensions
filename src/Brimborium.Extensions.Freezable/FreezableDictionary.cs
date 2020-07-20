namespace Brimborium.Extensions.Freezable {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class FreezableDictionary<TKey, TValue>
        : IFreezable
        , IDictionary<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> _Items;
        private int _IsFrozen;

        public FreezableDictionary() {
            this._Items = new Dictionary<TKey, TValue>();
        }

        public FreezableDictionary(IEqualityComparer<TKey> comparer) {
            this._Items = new Dictionary<TKey, TValue>(comparer);
        }

        public FreezableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
            if ((object)dictionary == null) {
                this._Items = new Dictionary<TKey, TValue>(comparer);
            } else {
                this._Items = new Dictionary<TKey, TValue>(dictionary, comparer);
            }
        }

        public TValue this[TKey key] {
            get {
                return this._Items[key];
            }

            set {
                this.ThrowIfFrozen();
                this._Items[key] = value;
            }
        }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)this._Items).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)this._Items).Values;

        public int Count => this._Items.Count;

        public bool IsReadOnly => (this._IsFrozen == 1);

        public void AddRange(IDictionary<TKey, TValue> values) {
            if (values is null) { return; }
            foreach (var item in values) {
                this[item.Key] = item.Value;
            }
        }

        public void Add(TKey key, TValue value) {
            this.ThrowIfFrozen();
            this._Items.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            this.ThrowIfFrozen();
            ((IDictionary<TKey, TValue>)this._Items).Add(item);
        }

        public void Clear() {
            this.ThrowIfFrozen();
            this._Items.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return ((IDictionary<TKey, TValue>)this._Items).Contains(item);
        }

        public bool ContainsKey(TKey key) {
            return this._Items.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            ((IDictionary<TKey, TValue>)this._Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return ((IDictionary<TKey, TValue>)this._Items).GetEnumerator();
        }

        public bool Remove(TKey key) {
            this.ThrowIfFrozen();
            return this._Items.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            this.ThrowIfFrozen();
            return ((IDictionary<TKey, TValue>)this._Items).Remove(item);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return this._Items.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IDictionary<TKey, TValue>)this._Items).GetEnumerator();
        }

        public bool Freeze() {
            if (System.Threading.Interlocked.CompareExchange(ref this._IsFrozen, 1, 0) == 0) {
                foreach (var item in this._Items.Values) {
                    if (item is IFreezable freezeable) {
                        freezeable.Freeze();
                    }
                }
                return true;
            } else {
                return false;
            }
        }

        public bool IsFrozen() => (this._IsFrozen == 1);

        public FreezableDictionary<TKey, TValue> Clone()
            => new FreezableDictionary<TKey, TValue>(this._Items, this._Items.Comparer);

        public TResult CopyValue<TResult>(Func<TValue, TValue> project)
            where TResult : FreezableDictionary<TKey, TValue>, new() {
            return this.CopyValue<TResult>(project, new TResult());
        }

        public TResult CopyValue<TResult>(Func<TValue, TValue> project, TResult target)
            where TResult : FreezableDictionary<TKey, TValue> {
            if ((object)target == null) { throw new ArgumentNullException(nameof(target)); }

            foreach (var kv in this._Items) {
                TValue value = ((object)project == null) ? kv.Value : project(kv.Value);
                if ((object)value == null) {
                    continue;
                } else {
                    target.Add(kv.Key, value);
                }
            }

            return target;
        }

        public TResult CopyKeyValue<TResult>(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> project)
            where TResult : FreezableDictionary<TKey, TValue>, new() {
            return this.CopyKeyValue<TResult>(project, new TResult());
        }

        public TResult CopyKeyValue<TResult>(Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> project, TResult target)
            where TResult : FreezableDictionary<TKey, TValue> {
            if ((object)target == null) { throw new ArgumentNullException(nameof(target)); }

            foreach (var kv in this._Items) {
                KeyValuePair<TKey, TValue> kvProjected = ((object)project == null) ? kv : project(kv);
                if ((object)kvProjected.Value == null) {
                    continue;
                } else {
                    target.Add(kvProjected.Key, kvProjected.Value);
                }
            }

            return target;
        }
    }

    public class FreezableDictionaryString<TValue> : FreezableDictionary<string, TValue> {
        public FreezableDictionaryString()
            : base(StringComparer.OrdinalIgnoreCase) {
        }

        public FreezableDictionaryString(IDictionary<string, TValue> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase) {
        }
    }
}
