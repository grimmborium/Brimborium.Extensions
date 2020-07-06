using System;
using System.Runtime.InteropServices;

namespace Brimborium.Extensions.Disposable {
    public static class InterlockedUtilty {
        public static TValue SetNextValue<TValue, TArg>(
                ref TValue currentValue,
                TArg arg,
                Func<TValue, TArg, TValue> getNextValue,
                Action<TValue> forget = null
            )
            where TValue : class {
            while (true) {
                TValue oldValue = currentValue;
                TValue nextValue = getNextValue(oldValue, arg);

                var prevValue = System.Threading.Interlocked.CompareExchange(
                    ref currentValue,
                    nextValue,
                    oldValue);
                if (ReferenceEquals(prevValue, oldValue)) {
                    return nextValue;
                }
                if (forget is object) {
                    forget(nextValue);
                }
            }
        }
        public static bool BitwiseSet(
            ref int currentValue,
            int bitValue) {
            if ((currentValue & bitValue) == bitValue) {
                return false;
            } else {
                while (true) {
                    var oldValue = currentValue;
                    var nextValue = oldValue | bitValue;
                    var prevValue = System.Threading.Interlocked.CompareExchange(
                        ref currentValue,
                        nextValue,
                        oldValue);
                    if (prevValue == oldValue) {
                        return ((prevValue & bitValue) != bitValue);
                    }
                }
            }
        }
        public static bool BitwiseClear(
            ref int currentValue,
            int bitValue) {
            if ((currentValue & bitValue) == 0) {
                return false;
            } else {
                while (true) {
                    var oldValue = currentValue;
                    var nextValue = oldValue & ~bitValue;
                    var prevValue = System.Threading.Interlocked.CompareExchange(
                        ref currentValue,
                        nextValue,
                        oldValue);
                    if (prevValue == oldValue) {
                        return (prevValue & bitValue) != 0;
                    }
                }
            }
        }
    }
}