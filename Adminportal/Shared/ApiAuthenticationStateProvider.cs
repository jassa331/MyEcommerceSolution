using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Threading.Tasks;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;

    public ApiAuthenticationStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

        ClaimsIdentity identity;
        if (!string.IsNullOrEmpty(token))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "User"),
                new Claim("jwt", token)
            };
            identity = new ClaimsIdentity(claims, "jwt");
        }
        else
        {
            identity = new ClaimsIdentity(); // empty = not authenticated
        }

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "User"),
            new Claim("jwt", token)
        };
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
