namespace Brimborium.Extensions.Freezable {
    using System;
    using System.Runtime.CompilerServices;

    public static class FreezableExtensions {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T AsFrozen<T>(this T that)
            where T : IFreezable {
            if (!that.IsFrozen()) { that.Freeze(); }
            return that;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowIfFrozen(this IFreezable that, string name = null) {
            if ((object)that != null) {
                if (that.IsFrozen()) {
                    if (name is null) {
                        throw new InvalidOperationException($"{that.GetType().FullName} is frozen.");
                    } else {
                        throw new InvalidOperationException($"{name} is frozen.");
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowIfNotFrozen(this IFreezable that, string name = null) {
            if ((object)that != null) {
                if (!(that.IsFrozen())) {
                    if (name is null) {
                        throw new InvalidOperationException($"{that.GetType().FullName} is NOT frozen.");
                    } else {
                        throw new InvalidOperationException($"{name} is NOT frozen.");
                    }
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetRefProperty<TProperty>(this IFreezable that, ref TProperty thisProperty, TProperty value)
            where TProperty : class {
            if (ReferenceEquals(thisProperty, value)) { return false; }
            that.ThrowIfFrozen();
            thisProperty = value;
            return true;
        }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetValueProperty<TProperty>(this IFreezable that, ref TProperty thisProperty, TProperty value)
            where TProperty : struct {
            if (thisProperty.Equals(value)) { return false; }
            that.ThrowIfFrozen();
            thisProperty = value;
            return true;
        }


        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetValueProperty<TProperty>(this IFreezable that, ref Nullable<TProperty> thisProperty, Nullable<TProperty> value)
            where TProperty : struct {
            if(thisProperty.HasValue && value.HasValue && thisProperty.Value.Equals(value.Value)) { return false; }
            if (!thisProperty.HasValue && !value.HasValue) { return false; }
            that.ThrowIfFrozen();
            thisProperty = value;
            return true;
        }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetStringProperty(this IFreezable that, ref string thisProperty, string value) {
            if (value == string.Empty) { value = null; }
            if (ReferenceEquals(thisProperty, value)) { return false; }
            if (string.Equals(thisProperty, value, System.StringComparison.Ordinal)) { return false; }
            that.ThrowIfFrozen();
            thisProperty = value;
            return true;
        }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetRefPropertyOnce<TProperty>(this IFreezable that, ref TProperty thisProperty, TProperty value, [CallerMemberName]string callerMemberName = null)
            where TProperty : class {
            if (ReferenceEquals(thisProperty, value)) { return false; }
            if ((object)thisProperty != null) {
                throw new System.ArgumentException($"{that.GetType().Name}.{callerMemberName} is already set.");
            }
            thisProperty = value;
            return true;
        }


#if later                
        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        protected bool SetOwner<TOwner>(ref TOwner thisPropertyOwner, TOwner value)
            where TOwner : class {
            if (ReferenceEquals(thisPropertyOwner, value)) { return false; }
            if (!(thisPropertyOwner is null)) { this.ThrowIfFrozen(); }
            thisPropertyOwner = value;
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetPropertyAndOwner<TThis, TProperty>(this TThis that, ref TProperty thisProperty, TProperty value)
            where TThis : class, IFreezable
            where TProperty : class, IObjectWithOwner<TThis> {
            if (ReferenceEquals(thisProperty, value)) { return false; }
            that.ThrowIfFrozen();
            var oldValue = thisProperty;
            thisProperty = value;
            if (!(value is null)) {
                value.Owner = that;
            }
            if (!(oldValue is null)) {
                if (ReferenceEquals(oldValue.Owner, that)) {
                    oldValue.Owner = null;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetOwnerAndProperty<TThis, TOwner>(this TThis that, ref TOwner thisProperty, TOwner value, Func<TOwner, TThis> getChildPropertyofOwner, Action<TOwner, TThis> setChildPropertyofOwner)
            where TThis : class, IFreezable
            where TOwner : class {
            if (ReferenceEquals(thisProperty, value)) { return false; }
            if (!(thisProperty is null)) { that.ThrowIfFrozen(); }
            var oldValue = thisProperty;
            thisProperty = value;
            if (!(value is null)) {
                setChildPropertyofOwner(value, that);
            }
            if (!(oldValue is null)) {
                if (ReferenceEquals(getChildPropertyofOwner(oldValue), that)) {
                    setChildPropertyofOwner(oldValue, null);
                }
            }
            return true;
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static TKey GetPairNameProperty<TThis, TKey, TValue>(this TThis @this, ref TKey thisKey, ref TValue thisValue, Func<TValue, TKey> getName)
            where TThis : class, IFreezable
            where TValue : class {
            if ((object)thisValue is null) {
                return thisKey;
            } else {
                return getName(thisValue);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetPairNameProperty<TThis, TKey, TValue>(this TThis @this, ref TKey thisKey, ref TValue thisValue, TKey value, Func<TThis, TKey, TValue> resolve)
            where TThis : class, IFreezable
            where TValue : class {
            @this.ThrowIfFrozen();
            thisKey = value;
            if ((object)value is null) {
                thisValue = default(TValue);
            } else {
                thisValue = resolve(@this, value);
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static TValue GetPairRefProperty<TThis, TKey, TValue>(this TThis @this, ref TKey thisKey, ref TValue thisValue, Func<TThis, TKey, TValue> resolve)
            where TThis : class, IFreezable
            where TKey : class
            where TValue : class {
            if (thisValue is null) {
                thisValue = resolve(@this, thisKey);
            }
            return thisValue;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool SetPairRefProperty<TThis, TKey, TValue>(this TThis @this, ref TKey thisKey, ref TValue thisValue, TValue value, Func<TValue, TKey> getName)
            where TThis : class, IFreezable
            where TKey : class
            where TValue : class {
            @this.ThrowIfFrozen();
            thisValue = value;
            if (value is null) {
                thisKey = default(TKey);
            } else {
                thisKey = getName(value);
            }
            return true;
        }

    public interface IObjectWithOwner<TOwner>
        where TOwner : class {
        TOwner Owner { get; set; }
    }
#endif
    }
}
