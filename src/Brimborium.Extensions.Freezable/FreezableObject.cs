namespace Brimborium.Extensions.Freezable {
    using System;
    using System.Runtime.CompilerServices;

    [System.Diagnostics.DebuggerStepThrough]
    public class FreezableObject : IFreezable {
        //[JsonIgnore]
        private int _IsFrozen;

        [System.Diagnostics.DebuggerStepThrough]
        public FreezableObject() { }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public virtual bool Freeze() {
            return (System.Threading.Interlocked.CompareExchange(ref this._IsFrozen, 1, 0) == 0);
        }

        [System.Diagnostics.DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public bool IsFrozen() => (this._IsFrozen == 1);
    }
}
