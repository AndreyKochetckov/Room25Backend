using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Room25Backend.Application.Interfaces;
using Room25Backend.Infrastructure.Services.Session;

namespace Room25Backend.Infrastructure.Services;

public static class ServicesExtentions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Подключаем Redis для кэша
        services.AddStackExchangeRedisCache(options => {
            options.Configuration = configuration["Redis:Host"];
            options.InstanceName = configuration["Redis:Name"];
        });

        // Сервисы
        services.AddTransient<ISessionService, SessionService>();
    }
}
