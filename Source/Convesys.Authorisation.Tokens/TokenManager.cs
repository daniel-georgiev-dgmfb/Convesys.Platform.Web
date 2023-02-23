using Twiligth.Kernel.Caching;
using Twiligth.Kernel.Logging;
using Twiligth.Kernel.Web;
using Twiligth.Kernel.Web.Authorisation;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Twiligth.Platform.Web.Tokens
{
    public class TokenManager : IBearerTokenManager
    {
        private readonly IHttpResourceRetriever _httpResourceRetriever;
        private readonly ICacheProvider _cacheProvider;
        private readonly IBearerTokenParser _bearerTokenParser;
        private readonly IEventLogger<TokenManager> _logger;

        public TokenManager(IHttpResourceRetriever httpResourceRetriever, ICacheProvider cacheProvider, IBearerTokenParser bearerTokenParser, IEventLogger<TokenManager> logger)
        {
            if (httpResourceRetriever == null)
                throw new ArgumentNullException(nameof(httpResourceRetriever));
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));
            if (bearerTokenParser == null)
                throw new ArgumentNullException(nameof(bearerTokenParser));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._httpResourceRetriever = httpResourceRetriever;
            this._bearerTokenParser = bearerTokenParser;
            this._cacheProvider = cacheProvider;
            this._logger = logger;
        }
        
        public async Task<TokenDescriptor> GetToken(IBearerTokenContext tokenContext, CancellationToken cancellationToken)
        {
            if (tokenContext == null)
                throw new ArgumentNullException(nameof(tokenContext));
            
            var key = tokenContext.ContextKey();

            this._logger.Log(SeverityLevel.Debug, 0, String.Format("Token request for context type: {0} received. Context key: {1}", tokenContext.GetType().FullName, key), (Exception)null, (s, e) => s.ToString());

            TokenDescriptor token;
            if (this._cacheProvider.TryGet<TokenDescriptor>(key, out token))
            {
                this._logger.Log(SeverityLevel.Info, 0, String.Format("Token with key: {0} found in cache.", key), (Exception)null, (s, e) => s.ToString());
                return token;
            }

            this._logger.Log(SeverityLevel.Info, 0, String.Format("Token with key: {0} not found in cache. Retrieving it from Central Authentication Service", key), (Exception)null, (s, e) => s.ToString());
            token = await this.GetTokenInternal(tokenContext, cancellationToken);
            if (token == null)
                return null;
            var cacheEntryOptions = new TokenCachePolicy(token);
            token = await this._cacheProvider.GetOrAddAsync<TokenDescriptor>(key, _ => Task.FromResult(token), cacheEntryOptions, cancellationToken);
            this._logger.Log(SeverityLevel.Info, 0, String.Format("Cache entry with key: {0} have been created. Entry absolute expiration set to: {1}", key, ((ICacheEntryOptions)cacheEntryOptions).AbsoluteExpiration), (Exception)null, (s, e) => s.ToString());
            return token;
        }

        protected virtual async Task<TokenDescriptor> GetTokenInternal(IBearerTokenContext tokenContext, CancellationToken cancellationToken)
        {
            if (tokenContext.Content == null)
                return null;
            TokenDescriptor token = null;
            try
            {
                this._httpResourceRetriever.HeadersHandler = tokenContext.HeaderHandler;
                var content = tokenContext.Content;
                var tokenEndpoint = tokenContext.Endpoint.ToString();
                this._logger.Log(SeverityLevel.Info, 0, String.Format("Retrieving token from Central Authentication Service endpoint: {0}", tokenContext.Endpoint), (Exception)null, (s, e) => s.ToString());
                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = content };
                var response = await this._httpResourceRetriever.SendAsync(request, cancellationToken);
                var tokenResponse = await response.Content.ReadAsStringAsync();
                this._logger.Log(SeverityLevel.Debug, 0, String.Format("Token raw response: {0}", tokenResponse), (Exception)null, (s, e) => s.ToString());
                if (response.IsSuccessStatusCode)
                {
                    token = await this.ParseToken(tokenResponse);
                    this._logger.Log(SeverityLevel.Debug, 0, String.Format("Token: {0}, Expire on: {1}", token != null ? token.Token : "null", token != null ? token.ExpireOn.ToString() : String.Empty), (Exception)null, (s, e) => s.ToString());
                }
            }
            catch(Exception ex)
            {
                this._logger.Log(SeverityLevel.Error, 0, String.Empty, ex, (s, e) => e.ToString());
            }
            return token;
        }

        private Task<TokenDescriptor> ParseToken(string tokenResponse)
        {
            return this._bearerTokenParser.Parse(tokenResponse);
        }
    }
}