using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Models; // For SchoolPortalNewContext
using SchoolPortal.Web.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Http;
using SchoolPortal.Web.Services;
using Microsoft.AspNetCore.ResponseCompression;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Services;
using SchoolPortal.API.Repositories;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Data;
using SchoolPortal.API.Data.Repositories;
using SchoolPortal.API.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.API.Mappings;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Add MVC controllers + views
// ----------------------------
builder.Services.AddControllersWithViews();

// ----------------------------
// Add session & context accessor
// ----------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "__Host-SchoolPortal.Session";
    options.Cookie.Path = "/";
});

// ----------------------------
// Add authentication with secure cookies
// ----------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "__Host-SchoolPortal.Auth";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Path = "/";
    });

// ----------------------------
// HTTPS redirection
// ----------------------------
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// ----------------------------
// Add CORS
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:7185",
                "https://your-production-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// ----------------------------
// Configure global API settings
// ----------------------------
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// ----------------------------
// Add HttpClients (properly structured)
// ----------------------------

// Configure HTTP clients with policies
// Default client with basic configuration
ConfigureHttpClient(builder.Services, "DefaultClient");

// Auth API client with retry and circuit breaker policies
ConfigureHttpClient(builder.Services, "AuthApi", true, true);

// General API client with retry policy
ConfigureHttpClient(builder.Services, "ApiClient", true, false);

// Helper method to configure HTTP clients
static void ConfigureHttpClient(IServiceCollection services, string name, bool addRetryPolicy = false, bool addCircuitBreaker = false)
{
    var builder = services.AddHttpClient(name, (serviceProvider, client) =>
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var baseUrl = config["ApiSettings:BaseUrl"] ?? 
            throw new InvalidOperationException("ApiSettings:BaseUrl not found in appsettings.json");
            
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    // Configure primary HTTP message handler
    builder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
        MaxConnectionsPerServer = 100,
        UseCookies = false,
        UseProxy = false,
        EnableMultipleHttp2Connections = true
    });

    // Add policies if requested
    if (addRetryPolicy || addCircuitBreaker)
    {
        if (addRetryPolicy)
        {
            // Fix for CS1929: Replace usage of serviceProvider.GetRequiredService<ILoggerFactory>() inside AddPolicyHandler lambda
            // The lambda parameter for AddPolicyHandler is of type HttpRequestMessage, not IServiceProvider.
            // To access IServiceProvider, use the overload that provides IServiceProvider as the first argument.

            builder.AddPolicyHandler((serviceProvider, request) => 
                Policy.Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500)
                    .WaitAndRetryAsync(
                        retryCount: 3,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (outcome, delay, retryCount, context) =>
                        {
                            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger<Program>();
                            logger.LogWarning(
                                "[Retry {RetryCount}] Delaying for {Delay}ms due to {StatusCode}",
                                retryCount,
                                delay.TotalMilliseconds,
                                outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message ?? "unknown error"
                            );
                        }));
        }
        
        if (addCircuitBreaker)
        {
            builder.AddPolicyHandler(GetCircuitBreakerPolicy());
        }
    }
}

// ----------------------------
// Configure cookie policy & antiforgery
// ----------------------------
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.CheckConsentNeeded = _ => false;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "__Host-Antiforgery";
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.HeaderName = "X-CSRF-TOKEN";
});

// Add these services before builder.Build()
builder.Services.AddMemoryCache(); // Add memory cache service
builder.Services.AddResponseCaching(); // Add response caching middleware

// Add the CachingService
builder.Services.AddScoped<ICachingService, CachingService>();
builder.Services.AddScoped<IClassSectionService, ClassSectionService>();
builder.Services.AddScoped<IClassSectionRepository, ClassSectionRepository>(); 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register EF Core DbContext (place this before repository / unit-of-work registrations)
builder.Services.AddDbContext<SchoolPortalNewContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Add it to appsettings.json.");
    options.UseSqlServer(connectionString);
});

// Register IUserRepository implementation (adjust concrete type/namespace if your repository class is named differently)
builder.Services.AddScoped<SchoolPortal.API.Interfaces.Repositories.IUserRepository, UserRepository>();

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Configure HTTP retry policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILoggerFactory loggerFactory)
{
    var logger = loggerFactory.CreateLogger("HttpPolicies");
    
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, delay, retryCount, context) =>
            {
                logger.LogWarning(
                    "[Retry {RetryCount}] Delaying for {Delay}ms due to {StatusCode}",
                    retryCount,
                    delay.TotalMilliseconds,
                    outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message ?? "unknown error"
                );
            });
}

// Configure HTTP circuit breaker policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (ex, breakDelay) =>
            {
                // Log circuit breaker state change
                var logger = new LoggerFactory().CreateLogger("HttpPolicies");
                logger.LogWarning("Circuit breaker opened for {BreakDelay}ms due to {Exception}", 
                    breakDelay.TotalMilliseconds, ex?.Exception.Message ?? "unknown error");
            },
            onReset: () =>
            {
                var logger = new LoggerFactory().CreateLogger("HttpPolicies");
                logger.LogInformation("Circuit breaker reset");
            },
            onHalfOpen: () =>
            {
                var logger = new LoggerFactory().CreateLogger("HttpPolicies");
                logger.LogInformation("Circuit breaker half-open");
            }
        );
}

// Update the HttpClient configuration to pass the logger factory
builder.Services.AddHttpClient("AuthApi", (serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings:BaseUrl not found");
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
    MaxConnectionsPerServer = 100,
    UseCookies = false,
    UseProxy = false,
    EnableMultipleHttp2Connections = true
})
.AddPolicyHandler((serviceProvider, request) =>
    // resolve ILoggerFactory from the runtime provider
    GetRetryPolicy(serviceProvider.GetRequiredService<ILoggerFactory>()))
.AddPolicyHandler(GetCircuitBreakerPolicy());

// ----------------------------
// Register AutoMapper (all assemblies)
// ----------------------------
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

// ----------------------------
// Build the app
// ----------------------------
var app = builder.Build();

// ----------------------------
// Middleware pipeline
// ----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Security headers
app.Use((context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

    if (context.Request.IsHttps)
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

    return next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
    endpoints.MapRazorPages();
});

app.UseCookiePolicy();
app.UseCors();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// In the app configuration section, after var app = builder.Build();
app.UseResponseCompression();
app.UseResponseCaching();

app.Run();
