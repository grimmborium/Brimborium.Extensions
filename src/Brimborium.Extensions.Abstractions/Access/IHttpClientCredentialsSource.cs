namespace Brimborium.Extensions.Access {
    public interface IHttpClientCredentialsSource
        : System.Net.ICredentials {
        string GetAuthenticationMode();
    }
}