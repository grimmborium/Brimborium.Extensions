namespace Brimborium.Extensions.SpreadScope {
    using System;

    /// <summary>
    /// Spread Scope Service
    /// </summary>
    public interface ISpreadScopeService : IDisposable {
        /// <summary>Gets the identifier.</summary>
        /// <returns>A identifier.</returns>
        long GetIdentifier();

        /// <summary>
        /// Ensure that this instance is bridged.
        /// </summary>
        void Accquire();

        /// <summary>Sets dirty.</summary>
        void SetDirty();

        /// <summary>Gets the service from the spread scope.</summary>
        /// <typeparam name="T">the service type</typeparam>
        /// <returns>a instance of Tor null</returns>
        T GetService<T>();

        /// <summary>Gets the required service from the spread scope.</summary>
        /// <typeparam name="T">the service type</typeparam>
        /// <returns>a instance of T</returns>
        T GetRequiredService<T>();

        /// <summary>Gets the application service provider.</summary>
        /// <value>The application service provider.</value>
        IServiceProvider ApplicationServiceProvider { get; }

        /// <summary>Gets the spread scoped service provider.</summary>
        /// <value>The spread scoped service provider.</value>
        IServiceProvider SpreadScopedServiceProvider { get; }

        /// <summary>Gets the scoped service provider.</summary>
        /// <value>The scoped service provider.</value>
        IServiceProvider ScopedServiceProvider { get; }
    }
}
