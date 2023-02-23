using Twiligth.Kernel.Logging;
using Twiligth.Kernel.Security.Validation;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Twiligth.HttpClient.Tests.L0.MockData
{
    internal class MockHttpClient : Twiligth.Platform.Web.HttpClient.HttpClient
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _onSend;
        //private readonly IBackchannelCertificateValidator _backchannelCertificateValidator;
        public MockHttpClient(IBackchannelCertificateValidator backchannelCertificateValidator, Func<HttpRequestMessage, Task<HttpResponseMessage>> onSend, IEventLogger<Twiligth.Platform.Web.HttpClient.HttpClient> logger) : base(backchannelCertificateValidator, logger)
        {
            this._onSend = onSend;
        }
        
        protected override HttpMessageHandler MessageHandler()
        {
            var handler = new MockHttpMessageHandler(this._onSend);
            handler.ServerCertificateCustomValidationCallback = base._backchannelCertificateValidator.Validate;
            return handler;
        }
    }
}