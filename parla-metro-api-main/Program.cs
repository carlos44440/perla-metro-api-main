using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using parla_metro_api_main.Services.HttpClients;
using parla_metro_api_main.Middlewares;
using parla_metro_api_main.Models.Requests;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// CORS para frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// JWT Authentication
var jwtKey =
    builder.Configuration["JWT:Secret"] ?? "your-super-secret-key-here-minimum-32-characters-long";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder
    .Services.AddAuthentication(options =>
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
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

// // HttpClient configurations para cada servicio
// builder.Services.AddHttpClient<IUsersClient, UsersClient>(client =>
// {
//     var baseUrl = builder.Configuration["Services:Users:BaseUrl"] ?? "http://localhost:5001";
//     client.BaseAddress = new Uri(baseUrl);
//     client.Timeout = TimeSpan.FromSeconds(30);
// });

// builder.Services.AddHttpClient<ITicketsClient, TicketsClient>(client =>
// {
//     var baseUrl = builder.Configuration["Services:Tickets:BaseUrl"] ?? "http://localhost:5002";
//     client.BaseAddress = new Uri(baseUrl);
//     client.Timeout = TimeSpan.FromSeconds(30);
// });

// builder.Services.AddHttpClient<IRoutesClient, RoutesClient>(client =>
// {
//     var baseUrl = builder.Configuration["Services:Routes:BaseUrl"] ?? "https://perla-metro-routes-service-wf9c.onrender.com";
//     client.BaseAddress = new Uri(baseUrl);
//     client.Timeout = TimeSpan.FromSeconds(30);
// });

 builder.Services.AddHttpClient<IStationsClient, StationsClient>(client =>
 {
     var baseUrl = builder.Configuration["Services:Stations:BaseUrl"] ?? "https://perla-metro-stations-service-zdgq.onrender.com";
     client.BaseAddress = new Uri(baseUrl);
     client.Timeout = TimeSpan.FromSeconds(30);
 });

// // Services
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IServiceOrchestrator, ServiceOrchestrator>();

// Controllers
builder.Services.AddControllers();

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new()
        {
            Title = "Perla Metro Main API",
            Version = "v1",
            Description = "API principal que orquesta todos los servicios del sistema Perla Metro",
        }
    );

    // JWT Configuration for Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Perla Metro Main API v1");
        c.RoutePrefix = "swagger";
    });
}

// Middleware pipeline
app.UseCors("AllowAll");

// Custom middlewares
// app.UseMiddleware<ErrorHandlingMiddleware>();
// app.UseMiddleware<LoggingMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers (deben ir antes de Ocelot)
app.MapControllers();

// Root endpoint
app.MapGet(
    "/",
    () =>
        Results.Ok(
            new
            {
                service = "Perla Metro Main API",
                version = "1.0.0",
                status = "running",
                features = new string[]
                {
                    "JWT Authentication",
                    "Service Orchestration",
                    "API Gateway (Ocelot)",
                    "CORS Support",
                    "Station Service Integration", 
                },
                endpoints = new string[]
                {
                    "/swagger",
                    "/auth/login",
                    "/api/users",
                    "/api/tickets",
                    "/api/routes",
                    "/api/stations",
                },
            }
        )
);

// Ocelot debe ir al final
await app.UseOcelot();

app.Run();
