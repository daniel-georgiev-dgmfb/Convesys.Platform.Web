using Twiligth.Kernel.Configuration;
using Twiligth.Kernel.Logging;
using Twiligth.Kernel.Security.Validation;
using Twiligth.Kernel.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Twiligth.Platform.Web.HttpClient
{
    public class HttpClient : IHttpResourceRetriever
    {
        private readonly IEventLogger<HttpClient> _logger;
        internal protected readonly IBackchannelCertificateValidator _backchannelCertificateValidator;
        static HttpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
        public HttpClient(IBackchannelCertificateValidator backchannelCertificateValidator)
        {
            this._backchannelCertificateValidator = backchannelCertificateValidator;
        }

        public bool RequireHttps { get; set; }
        public TimeSpan Timeout { get; set; }
        public long MaxResponseContentBufferSize { get; set; }
        public ICustomConfigurator<IHttpResourceRetriever> HttpDocumentRetrieverConfigurator { private get; set; }
        public Action<HttpRequestHeaders> HeadersHandler { private get; set; }

        public ICollection<Cookie> Cookies { get; }

        /// <summary>
        /// Initialise an instance of Http document retriever
        /// </summary>
        /// <param name="backchannelCertificateValidator"></param>
        public HttpClient(IBackchannelCertificateValidator backchannelCertificateValidator, IEventLogger<HttpClient> logger)
        {
            if (backchannelCertificateValidator == null)
                throw new ArgumentNullException("backchannelCertificateValidator");
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._backchannelCertificateValidator = backchannelCertificateValidator;
            this._logger = logger;
            this.Timeout = TimeSpan.FromSeconds(30);
            this.MaxResponseContentBufferSize = 10485760L;
            this.RequireHttps = true;
            this.HeadersHandler = _ => { };
            this.Cookies = new HashSet<Cookie>();
        }

        /// <summary>
        /// Retrieve a detadata document from the web
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string address, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address");

            if (this.HttpDocumentRetrieverConfigurator != null)
            {
                this.HttpDocumentRetrieverConfigurator.Configure(this);
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Custom configuration appied. RequireHttps set to: {0}, Timeout set to: {1}, MaxResponseContentBufferSize set to: {2}", this.RequireHttps, this.Timeout, this.MaxResponseContentBufferSize), (Exception)null, (s, e) => s.ToString());
            }

            if (this.RequireHttps && !Utility.IsHttps(address))
                throw new ArgumentException(string.Format("IDX10108: The address specified '{0}' is not valid as per HTTPS scheme. Please specify an https address for security reasons. If you want to test with http address, set the RequireHttps property  on IDocumentRetriever to false.", (object)address), "address");

            string str1;
            try
            {
                using (var httpClient = this.GetHttpClient())
                {
                    var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, address), cancel)
                       .ConfigureAwait(false);
                    var response = httpResponseMessage;
                    httpResponseMessage = null;
                    response.EnsureSuccessStatusCode();
                    var str = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                    str1 = str;
                }
                
            }
            catch (Exception ex)
            {
                this._logger.Log(SeverityLevel.Error, 0, String.Empty, ex, (s, e) => e.ToString());
                throw new IOException(String.Format("IDX10804: Unable to retrieve resource from: '{0}'.", address), ex);
            }
            return str1;
        }

        public async Task<string> PutAsyncAsJson(string address, string contentValue, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address");

            if (this.HttpDocumentRetrieverConfigurator != null)
            {
                this.HttpDocumentRetrieverConfigurator.Configure(this);
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Custom configuration appied. RequireHttps set to: {0}, Timeout set to: {1}, MaxResponseContentBufferSize set to: {2}", this.RequireHttps, this.Timeout, this.MaxResponseContentBufferSize), (Exception)null, (s, e) => s.ToString());
            }

            if (this.RequireHttps && !Utility.IsHttps(address))
                throw new ArgumentException(string.Format("IDX10108: The address specified '{0}' is not valid as per HTTPS scheme. Please specify an https address for security reasons. If you want to test with http address, set the RequireHttps property  on IDocumentRetriever to false.", (object)address), "address");

            string str1;
            try
            {
                using (var httpClient = this.GetHttpClient())
                {
                    var content = new StringContent(contentValue, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Put, address) { Content = content }, cancel)
                       .ConfigureAwait(false);
                    var response = httpResponseMessage;
                    httpResponseMessage = null;
                    response.EnsureSuccessStatusCode();
                    var str = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                    str1 = str;
                }
                
            }
            catch (Exception ex)
            {
                this._logger.Log(SeverityLevel.Error, 0, String.Empty, ex, (s, e) => e.ToString());
                throw new IOException(String.Format("IDX10804: Unable to retrieve resource from: '{0}'.", address), ex);
            }
            return str1;
        }

        public async Task<string> PostAsyncAsJson(string address, string contentValue, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException("address");

            if (this.HttpDocumentRetrieverConfigurator != null)
            {
                this.HttpDocumentRetrieverConfigurator.Configure(this);
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Custom configuration appied. RequireHttps set to: {0}, Timeout set to: {1}, MaxResponseContentBufferSize set to: {2}", this.RequireHttps, this.Timeout, this.MaxResponseContentBufferSize), (Exception)null, (s, e) => s.ToString());
            }

            if (this.RequireHttps && !Utility.IsHttps(address))
                throw new ArgumentException(string.Format("IDX10108: The address specified '{0}' is not valid as per HTTPS scheme. Please specify an https address for security reasons. If you want to test with http address, set the RequireHttps property  on IDocumentRetriever to false.", (object)address), "address");

            string str1;
            try
            {
                using (var httpClient = this.GetHttpClient())
                {
                    var content = new StringContent(contentValue, Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, address) { Content = content }, cancel)
                           .ConfigureAwait(false);
                    var response = httpResponseMessage;
                    httpResponseMessage = null;
                    response.EnsureSuccessStatusCode();
                    var str = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                    str1 = str;
                }
            }
            catch (Exception ex)
            {
                this._logger.Log(SeverityLevel.Error, 0, String.Empty, ex, (s, e) => e.ToString());
                throw new IOException(String.Format("IDX10804: Unable to retrieve resource from: '{0}'.", address), ex);
            }
            return str1;
        }

        async Task<HttpResponseMessage> IResourceRetriever.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool throwIfNotSuccess = true)
        {
            if (this.HttpDocumentRetrieverConfigurator != null)
            {
                this.HttpDocumentRetrieverConfigurator.Configure(this);
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Custom configuration appied. RequireHttps set to: {0}, Timeout set to: {1}, MaxResponseContentBufferSize set to: {2}", this.RequireHttps, this.Timeout, this.MaxResponseContentBufferSize), (Exception)null, (s, e) => s.ToString());
            }

            if (this.RequireHttps && !Utility.IsHttps(request.RequestUri))
                throw new ArgumentException(string.Format("IDX10108: The address specified '{0}' is not valid as per HTTPS scheme. Please specify an https address for security reasons. If you want to test with http address, set the RequireHttps property  on IDocumentRetriever to false.", (object)request.RequestUri), "address");
            
            using (var httpClient = this.GetHttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                var httpResponseMessage = await httpClient.SendAsync(request, cancellationToken)
                   .ConfigureAwait(false);
                var response = httpResponseMessage;
                httpResponseMessage = null;
                if(throwIfNotSuccess)
                    response.EnsureSuccessStatusCode();
                return response;
            }
        }

        protected virtual System.Net.Http.HttpClient GetHttpClient()
        {
            var messageHandler = this.MessageHandler();
            var httpClient = new System.Net.Http.HttpClient(messageHandler)
            {
                Timeout = this.Timeout,
                MaxResponseContentBufferSize = this.MaxResponseContentBufferSize
            };

            if (this.HeadersHandler != null)
            {
                this.HeadersHandler(httpClient.DefaultRequestHeaders);
                this._logger.Log(SeverityLevel.Trace, 0, String.Format("Headers handler invoked. authorisaton header: {0}", httpClient.DefaultRequestHeaders.Authorization), (Exception)null, (s, e) => s.ToString());
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Headers handler invoked. Headers: {0}", httpClient.DefaultRequestHeaders.ToString()), (Exception)null, (s, e) => s.ToString());
            }
            return httpClient;
        }

        protected virtual HttpMessageHandler MessageHandler()
        {
            var messageHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = this._backchannelCertificateValidator.Validate
            };
            return messageHandler;
        }
    }
}