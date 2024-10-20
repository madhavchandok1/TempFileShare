using Microsoft.Extensions.DependencyInjection;
using TempFileShare.Contracts.Interfaces.Repositories;

namespace TempFileShare.Repositories.Extension
{
    public static class RepositoriesExtension
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            _ = services.AddScoped<ISessionRepository, SessionRepository>();
            //builder.Services.AddScoped<IGuestAccessRepository, GuestAccessRepository>();

            return services;
        }
    }
}
