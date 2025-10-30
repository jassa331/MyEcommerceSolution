using Microsoft.JSInterop;

namespace UserPortal.handlerof_token
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IJSRuntime _js;
        public JwtAuthorizationHandler(IJSRuntime js)
        {
            _js = js;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
