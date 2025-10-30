using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Adminportal.Services
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _js;

        public TokenHandler(IJSRuntime js)
        {
            _js = js;
        }

        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }

        //    return await base.SendAsync(request, cancellationToken);
        //}
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
