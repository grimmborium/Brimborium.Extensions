using System;
using Xunit;

namespace Brimborium.Extensions.Http.Test {
    public class Usage {
        [Fact]
        public void Usage1() {
            var sut = new HttpClientGenerator();
            var cfg1 = new HttpClientConfiguration();
            var cfg2 = new HttpClientConfiguration();
            var client1 = sut.CreateHttpClient(cfg1);
            var client2 = sut.CreateHttpClient(cfg2);
            Assert.NotSame(client1, client2);
        }
        [Fact]
        public void Usage2() {
            var sut = new HttpClientGenerator();
            var cfg1 = new HttpClientConfiguration() { BaseUrl= "https://www.wikipedia.org/" };
            var cfg2 = new HttpClientConfiguration() { BaseUrl = "https://www.wikipedia.org/" };
            var clientCaretaker1 = sut.GetHttpClient(cfg1);
            clientCaretaker1.Dispose();
            var clientCaretaker2 = sut.GetHttpClient(cfg2);            
            clientCaretaker2.Dispose();
            Assert.Same(clientCaretaker1.HttpClient, clientCaretaker2.HttpClient);

            cfg2.BaseUrl = "https://localhost";
            var clientCaretaker3 = sut.GetHttpClient(cfg2);
            Assert.NotSame(clientCaretaker2.HttpClient, clientCaretaker3.HttpClient);
            clientCaretaker3.Dispose();
        }
    }
}
