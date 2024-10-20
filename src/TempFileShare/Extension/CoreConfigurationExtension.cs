using TempFileShare.DataAccessLayer.Extension;
using TempFileShare.Repositories.Extension;
using TempFileShare.Utilities.Extension;

namespace TempFileShare.Extension
{
    public static class CoreConfigurationExtension
    {
        public static IServiceCollection AddCoreConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddControllerServices();
            _ = services.AddDALServices(configuration);
            _ = services.AddRepositoryServices();
            _ = services.AddUtilityServices();

            return services;
        }
    }
}
