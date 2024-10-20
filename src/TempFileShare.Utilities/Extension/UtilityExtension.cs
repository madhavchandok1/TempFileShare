using Microsoft.Extensions.DependencyInjection;
using TempFileShare.Contracts.Interfaces.Utilities;
using TempFileShare.Utilities.Token;

namespace TempFileShare.Utilities.Extension
{
    public static class UtilityExtension
    {
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            _ = services.AddScoped<ITokenService, TokenService>();

            _ = services.AddScoped<IBlobStorage, BlobStorage>();

            return services;
        }
    }
}
