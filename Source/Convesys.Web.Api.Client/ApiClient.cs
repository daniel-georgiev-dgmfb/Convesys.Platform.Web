using Convesys.Kernel.Configuration;
using Convesys.Kernel.Logging;
using Convesys.Kernel.Web;
using Convesys.Kernel.Web.Authorisation;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Convesys.Platform.Web.Api.Client
{
    public class ApiClient : IApiClient
    {
        private readonly IBearerTokenManager _bearerTokenManager;
        private readonly IHttpResourceRetriever _httpClient;
        private readonly IEventLogger<ApiClient> _logger;
        private readonly ICustomConfigurator<IHttpResourceRetriever> _customConfigurator;

        public ApiClient(IBearerTokenManager bearerTokenManager, IHttpResourceRetriever httpClient, IEventLogger<ApiClient> logger, ICustomConfigurator<IHttpResourceRetriever> customConfigurator = null)
        {
            _bearerTokenManager = bearerTokenManager ?? throw new ArgumentNullException(nameof(bearerTokenManager));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _customConfigurator = customConfigurator;
        }

        public async Task<string> GetAsync(RequestContext request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            this.Configure();

            await this.Authorizaton(request, cancellationToken);
            return await this._httpClient.GetAsync(request.ResourceEndpoint.Endpont.AbsoluteUri, cancellationToken);
        }

        public async Task<string> PostAsyncAsJson(RequestContext request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentNullException(nameof(request.Content));

            this.Configure();
            await this.Authorizaton(request, cancellationToken);
            return await this._httpClient.PostAsyncAsJson(request.ResourceEndpoint.Endpont.AbsoluteUri, request.Content, cancellationToken);
        }

        public async Task<string> PutAsyncAsJson(RequestContext request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentNullException(nameof(request.Content));

            this.Configure();
            await this.Authorizaton(request, cancellationToken);
            return await this._httpClient.PutAsyncAsJson(request.ResourceEndpoint.Endpont.AbsoluteUri, request.Content, cancellationToken);
        }

        async Task<HttpResponseMessage> IApiClient.SendAsync(RequestContext request, HttpMethod method, CancellationToken cancellationToken, bool throwIfNotSuccess)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if ((method == HttpMethod.Post || method == HttpMethod.Put) &&  string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentNullException(nameof(request.Content));
            this.Configure();
            await this.Authorizaton(request, cancellationToken);
            var requestMessage = new HttpRequestMessage(method, request.ResourceEndpoint.Endpont);
            if (method == HttpMethod.Post || method == HttpMethod.Put)
                requestMessage.Content = new StringContent(request.Content);

            var response = await this._httpClient.SendAsync(requestMessage, cancellationToken, throwIfNotSuccess);
            return response;
        }

        private async Task Authorizaton(RequestContext request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.ClientCredentials == null)
                {
                    _httpClient.HeadersHandler = _ => { _.Authorization = null; };
                    _logger.Log(SeverityLevel.Info, 0, "Null Authorisation header set.", null,
                        (s, e) => s.ToString());

                    return;
                }
                
                _logger.Log(SeverityLevel.Info, 0, "Begin acquiring token.", null, (s, e) => s.ToString());

                var token = await _bearerTokenManager.GetToken(request.ClientCredentials, cancellationToken);

                if (token == null)
                {
                    _logger.Log(SeverityLevel.Debug, 0, "Token: not acquired. Anonymous request", null,
                        (s, e) => s.ToString());
                    _httpClient.HeadersHandler = _ => { _.Authorization = null; };

                    return;
                }
                
                _httpClient.HeadersHandler = h =>
                    h.Authorization = new AuthenticationHeaderValue(token.TokenType, token.Token);
                _logger.Log(SeverityLevel.Info, 0, "Authorisation header set.", null, (s, e) => s.ToString());
            }
            catch (HttpRequestException e)
            {
                this._logger.Log(SeverityLevel.Error, 0, string.Empty, e, (s, ex) => ex.ToString());
                throw;
            }
        }

        private void Configure()
        {
            if (this._customConfigurator != null)
                this._customConfigurator.Configure(this._httpClient);
        }
    }
}