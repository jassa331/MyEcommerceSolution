using Admin.API.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Database Connection
builder.Services.AddDbContext<admindbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Enable CORS (Allow Admin & User portals)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7215", // Admin Portal
                "https://localhost:7171"  // User Portal
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Authorization");
    });
});

// ✅ JWT Authentication Setup
// ✅ JWT Authentication Setup
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyString = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.");
var keyBytes = Encoding.UTF8.GetBytes(keyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ Token validation failed: {context.Exception.Message}");
            if (context.Exception.InnerException != null)
                Console.WriteLine($"➡ Inner: {context.Exception.InnerException.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"✅ Token validated successfully for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});
Console.WriteLine($"AdminAPI Key: {builder.Configuration["Jwt:Key"]}");


// ✅ Log JWT Settings for Debugging
Console.WriteLine("🟢 --- JWT SETTINGS ---");
Console.WriteLine($"🔑 Key Length: {jwtSection["Key"]?.Length}");
Console.WriteLine($"📤 Issuer: {jwtSection["Issuer"]}");
Console.WriteLine($"📥 Audience: {jwtSection["Audience"]}");
Console.WriteLine("🟢 --------------------");

// ✅ Authorization Policy (Optional)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("IsAdmin", "True"));
});

// ✅ Swagger Setup with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Admin API",
        Version = "v1",
        Description = "Admin Management API for MyEcommerceSolution"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {your JWT token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ MVC Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ✅ Swagger UI (Only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Serve Static Files (images, etc.)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "/static"
});

// ✅ Middleware Pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();


// ✅ Map Controllers
app.MapControllers();

// ✅ Run Application
app.Run();
