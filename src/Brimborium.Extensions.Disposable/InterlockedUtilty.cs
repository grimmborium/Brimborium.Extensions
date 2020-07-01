using System;

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

                var prevDict = System.Threading.Interlocked.CompareExchange(
                    ref currentValue,
                    nextValue,
                    oldValue);
                if (ReferenceEquals(prevDict, oldValue)) {
                    return nextValue;
                }
                if (forget is object) {
                    forget(nextValue);
                }
            }
        }
    }
}