using Adminportal;
using Adminportal.Services; // ? for TokenHandler
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ? Register TokenHandler
builder.Services.AddScoped<TokenHandler>();

// ? Configure HttpClient with inner handler (IMPORTANT)
builder.Services.AddScoped(sp =>
{
    var js = sp.GetRequiredService<IJSRuntime>();

    // create handler and set inner handler
    var handler = new TokenHandler(js)
    {
        InnerHandler = new HttpClientHandler() // ?? FIX — prevents "inner handler not assigned"
    };

    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7255/") // Admin API base URL
    };

    return client;
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

await builder.Build().RunAsync();
