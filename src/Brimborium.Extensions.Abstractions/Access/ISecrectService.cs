namespace Brimborium.Extensions.Access {
    /// <summary>Provide a service that helps with secrets.</summary>
    public interface ISecrectService {
        /// <summary>Resolve the user+password by the <see cref="IUnifiedConnectionString.SecretKey"/></summary>
        /// <param name="connectionString">a connectionstring</param>
        /// <returns>a copy of the connectionstring secretkey resolved.</returns>
        IUnifiedConnectionString Resolve(IUnifiedConnectionString connectionString);

        // TODO: think of CRUD??
    }
}
