using Admin.API.DAL;
using Admin.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

public class ApiLogAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;

        var method = http.Request.Method;
        var path = http.Request.Path;
        var ip = http.Connection.RemoteIpAddress?.ToString();

        // UserId from JWT
        Guid? userId = null;
        var userIdClaim = http.User.FindFirst("UserId")?.Value;
        if (!string.IsNullOrEmpty(userIdClaim))
            userId = Guid.Parse(userIdClaim);

        // Request Body
        var requestBody = System.Text.Json.JsonSerializer.Serialize(context.ActionArguments);

        // Execute API
        var resultContext = await next();

        // Response
        string responseBody = "";
        int statusCode = http.Response.StatusCode;

        if (resultContext.Result is ObjectResult objResult)
        {
            responseBody = System.Text.Json.JsonSerializer.Serialize(objResult.Value);
            statusCode = objResult.StatusCode ?? 200;
        }

        // ✅ Using scope properly
        using var scope = http.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<admindbcontext>();

        var log = new ApiLog
        {
            UserId = userId,
            IpAddress = ip,
            Method = method,
            Path = path,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            StatusCode = statusCode,
            CreatedAt = DateTime.UtcNow   // <-- always set
        };


        await db.ApiLogs.AddAsync(log);
        await db.SaveChangesAsync();
    }
}