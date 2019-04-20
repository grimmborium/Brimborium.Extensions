namespace Brimborium.Extensions.SpreadScope {
    /// <summary>Used in the dependency injection constructors.</summary>
    /// <typeparam name="T">The dependency service</typeparam>
    public interface ISpreadScoped<T>
        where T : class {
        /// <summary>Gets the service instance.</summary>
        T Instance { get; }
    }
}
