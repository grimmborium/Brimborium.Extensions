namespace Brimborium.Extensions.Access {
    using System.Net;
    using System.Threading.Tasks;

    public interface IHttpWebRequestCredentials : ICredentials {
        void SetCredentials(HttpWebRequest request, string url);

        Task PrepareAsync(string url);

        Task SetCredentialsAsync(HttpWebRequest request, string url);
    }
}
