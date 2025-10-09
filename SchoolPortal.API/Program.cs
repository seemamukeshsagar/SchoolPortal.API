using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;
using SchoolPortal.API.Repositories;
using SchoolPortal.API.Services;
using System.Reflection;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.API.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext with connection string from configuration
builder.Services.AddDbContext<SchoolPortalNewContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Add AutoMapper with profiles
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CompanyProfile>();
}, typeof(Program).Assembly);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "School Portal API", 
        Version = "v1",
        Description = "API for School Portal Management System",
        Contact = new OpenApiContact
        {
            Name = "School Portal Support",
            Email = "support@schoolportal.com",
            Url = new Uri("https://schoolportal.com/support")
        },
        License = new OpenApiLicense
        {
            Name = "School Portal License",
            Url = new Uri("https://schoolportal.com/license")
        }
    });
    
    // Add XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    // Enable annotations for better API documentation
    c.EnableAnnotations();
});

// Add CORS services with a development policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllForDevelopment", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Portal API v1"));
}

app.UseHttpsRedirection();

// Enable CORS with the development policy
app.UseCors("AllowAllForDevelopment");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
