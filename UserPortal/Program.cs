using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using UserPortal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp =>
{
    var js = sp.GetRequiredService<IJSRuntime>();
    var http = new HttpClient { BaseAddress = new Uri("https://localhost:7066/") };

    // attach token dynamically
    js.InvokeAsync<string>("localStorage.getItem", "authToken").AsTask().ContinueWith(t =>
    {
        if (!string.IsNullOrEmpty(t.Result))
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", t.Result);
    });

    return http;
});

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://localhost:7117/") };
    return client;
});

await builder.Build().RunAsync();
