using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartInvestigation.Application;
using SmartInvestigation.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Render Port Binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://*:{port}");

// ── Layer DI Registration ──
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Controllers ──
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ── JWT Authentication ──
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyThatIsAtLeast32Characters!!")),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── Rate Limiting ──
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 10;
    });
    options.RejectionStatusCode = 429;
});

// ── CORS ──
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ── Swagger ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Smart Investigation & Case Management API",
        Version = "v1",
        Description = "Production-grade API for law enforcement investigation and case management",
        Contact = new OpenApiContact { Name = "Admin", Email = "admin@smartinvestigation.gov" }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // Custom Schema ID Selector to avoid 500 errors from naming collisions
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Root routes
app.MapGet("/", () => "Smart Investigation API is Live and Running!");
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// Exception Logging Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[CRASH]: {ex.GetType().Name} - {ex.Message}");
        Console.WriteLine(ex.StackTrace);

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            Console.WriteLine("[SWAGGER ERROR]: Failed to generate/serve OpenAPI documentation.");
        }
        throw;
    }
});

// Seed Database
try 
{
    Console.WriteLine("--- Starting Database Seeding ---");
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<SmartInvestigation.Infrastructure.Persistence.AppDbContext>();
        await SmartInvestigation.Infrastructure.Persistence.SeedData.DataSeeder.SeedAsync(context);
    }
    Console.WriteLine("--- Database Seeding Completed Successfully ---");
}
catch (Exception ex)
{
    Console.WriteLine($"--- Database Seeding Failed: {ex.Message} ---");
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("v1/swagger.json", "Smart Investigation API v1"); // Relative path for Render
    c.RoutePrefix = "swagger"; 
});

// app.UseHttpsRedirection(); // SSL handled at gateway
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine($"--- Application is starting on port {port} ---");

app.Run();
