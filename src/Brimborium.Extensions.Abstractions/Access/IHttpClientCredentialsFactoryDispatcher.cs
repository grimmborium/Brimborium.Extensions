namespace Brimborium.Extensions.Access {
    /// <summary>
    /// Dispatch by <see cref="IHttpClientCredentialsFactory.GetAuthenticationMode"/> to the implementation.
    /// </summary>
    /// <seealso cref="IHttpClientCredentialsFactory" />
    public interface IHttpClientCredentialsFactoryDispatcher 
        : IHttpClientCredentialsFactory {
        // Can we use a generic solution? -> various connection strings types will be great.
    }
}
