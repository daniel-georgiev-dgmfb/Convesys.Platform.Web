using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.HttpClient.Tests.L0.MockData
{
    internal class MockHttpMessageHandler : HttpClientHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _onSend;
        public MockHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> onSend)
        {
            this._onSend = onSend;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return this._onSend(request);
        }
    }
}