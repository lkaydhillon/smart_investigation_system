using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInvestigation.Domain.Interfaces;
using SmartInvestigation.Infrastructure.Persistence;
using SmartInvestigation.Infrastructure.Repositories;
using SmartInvestigation.Infrastructure.Services;

namespace SmartInvestigation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Persistence.Interceptors.AuditInterceptor>();

        // Database
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<Persistence.Interceptors.AuditInterceptor>();
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
                   .AddInterceptors(interceptor);
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddSingleton<GeminiAIService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<SmartInvestigation.Application.Interfaces.ILegalRecommendationService, LegalRecommendationService>();
        services.AddScoped<SmartInvestigation.Application.Interfaces.IDocumentGenerationService, DocumentGenerationService>();

        return services;
    }
}
