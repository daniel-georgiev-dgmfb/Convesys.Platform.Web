using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Twilight.Platform.Web.Api.Client
{
    public interface IApiClient
    {
        Task<string> GetAsync(RequestContext request, CancellationToken cancellationToken);
        Task<string> PostAsyncAsJson(RequestContext request, CancellationToken cancellationToken);
        Task<string> PutAsyncAsJson(RequestContext request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> SendAsync(RequestContext request, HttpMethod method, CancellationToken cancellationToken, bool throwIfNotSuccess);
    }
}