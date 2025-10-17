using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using SchoolPortal.API.Data;
using SchoolPortal.API.Data.Repositories;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Mappings;
using SchoolPortal.API.Models;
using SchoolPortal.API.Repositories;
using SchoolPortal.API.Services;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Services;

// ---------------------------------------------------------
// Create WebApplication Builder
// ---------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// MVC & Razor Views
// ---------------------------------------------------------
builder.Services.AddControllersWithViews();

// ---------------------------------------------------------
// Session & Context Accessor
// ---------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.Name = "__Host-SchoolPortal.Session";
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.Path = "/";
});

// ---------------------------------------------------------
// Authentication (Secure Cookie-Based)
// ---------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";
		options.LogoutPath = "/Account/Logout";
		options.AccessDeniedPath = "/Account/AccessDenied";
		options.ExpireTimeSpan = TimeSpan.FromDays(30);
		options.SlidingExpiration = true;

		options.Cookie.Name = "__Host-SchoolPortal.Auth";
		options.Cookie.HttpOnly = true;
		options.Cookie.IsEssential = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.SameSite = SameSiteMode.Lax;
		options.Cookie.Path = "/";
	});

// ---------------------------------------------------------
// HTTPS Redirection
// ---------------------------------------------------------
builder.Services.AddHttpsRedirection(options =>
{
	options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
	options.HttpsPort = 443;
});

// ---------------------------------------------------------
// CORS Configuration
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("https://localhost:5001", "https://your-production-domain.com")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials()
			  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
	});
});

// ---------------------------------------------------------
// Configuration: ApiSettings
// ---------------------------------------------------------
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// ---------------------------------------------------------
// Database Context (EF Core)
// ---------------------------------------------------------
builder.Services.AddDbContext<SchoolPortalNewContext>(options =>
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
		?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
	options.UseSqlServer(connectionString);
});

// ---------------------------------------------------------
// Caching & Compression
// ---------------------------------------------------------
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression(options =>
{
	options.EnableForHttps = true;
	options.Providers.Add<BrotliCompressionProvider>();
	options.Providers.Add<GzipCompressionProvider>();
});

// ---------------------------------------------------------
// Antiforgery & Cookie Policy
// ---------------------------------------------------------
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
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.Path = "/";
	options.HeaderName = "X-CSRF-TOKEN";
});

// ---------------------------------------------------------
// Dependency Injection (Services & Repositories)
// ---------------------------------------------------------
builder.Services.AddScoped<ICachingService, CachingService>();
builder.Services.AddScoped<IClassSectionService, ClassSectionService>();
builder.Services.AddScoped<IClassSectionRepository, ClassSectionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<ILoginAttemptService, LoginAttemptService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

// ---------------------------------------------------------
// AutoMapper
// ---------------------------------------------------------
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AutoMapperProfile>();
});

// ---------------------------------------------------------
// HTTP Clients (with Polly policies)
// ---------------------------------------------------------
ConfigureHttpClient(builder.Services, "DefaultClient");
ConfigureHttpClient(builder.Services, "AuthApi", addRetryPolicy: true, addCircuitBreaker: true);
ConfigureHttpClient(builder.Services, "ApiClient", addRetryPolicy: true);

static void ConfigureHttpClient(IServiceCollection services, string name, bool addRetryPolicy = false, bool addCircuitBreaker = false)
{
	var builder = services.AddHttpClient(name, (serviceProvider, client) =>
	{
		var config = serviceProvider.GetRequiredService<IConfiguration>();
		var baseUrl = config["ApiSettings:BaseUrl"]
			?? throw new InvalidOperationException("ApiSettings:BaseUrl not configured in appsettings.json.");

		client.BaseAddress = new Uri(baseUrl);
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.Timeout = TimeSpan.FromSeconds(30);
	});

	builder.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
	{
		PooledConnectionLifetime = TimeSpan.FromMinutes(5),
		PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
		MaxConnectionsPerServer = 100,
		UseCookies = false,
		UseProxy = false,
		EnableMultipleHttp2Connections = true
	});

	if (addRetryPolicy)
		builder.AddPolicyHandler((sp, _) => GetRetryPolicy(sp.GetRequiredService<ILoggerFactory>()));

	if (addCircuitBreaker)
		builder.AddPolicyHandler(GetCircuitBreakerPolicy());
}

// ---------------------------------------------------------
// Polly Policies (Resilient HTTP)
// ---------------------------------------------------------
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILoggerFactory loggerFactory)
{
	var logger = loggerFactory.CreateLogger("HttpPolicies");
	return HttpPolicyExtensions
		.HandleTransientHttpError()
		.OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
		.WaitAndRetryAsync(3,
			retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
			(outcome, delay, retryCount, context) =>
			{
				logger.LogWarning("[Retry {RetryCount}] Delay {Delay}ms - Reason: {Reason}",
					retryCount, delay.TotalMilliseconds,
					outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message ?? "unknown");
			});
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
	var logger = new LoggerFactory().CreateLogger("HttpPolicies");

	return HttpPolicyExtensions
		.HandleTransientHttpError()
		.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
			onBreak: (ex, delay) => logger.LogWarning("Circuit opened for {Delay}ms: {Message}", delay.TotalMilliseconds, ex.Exception?.Message),
			onReset: () => logger.LogInformation("Circuit reset"),
			onHalfOpen: () => logger.LogInformation("Circuit half-open"));
}

// ---------------------------------------------------------
// Build Application
// ---------------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
}

// Security Headers
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
app.UseCookiePolicy();
app.UseCors();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();
app.UseResponseCaching();

// Default Route
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
