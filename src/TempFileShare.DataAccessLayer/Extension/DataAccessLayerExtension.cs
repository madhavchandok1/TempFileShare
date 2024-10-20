using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using TempFileShare.Contracts.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TempFileShare.DataAccessLayer.Extension
{
    public static class DataAccessLayerExtension
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<ApplicationDBContext>(options =>
                {
                    _ = options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
                });

            _ = services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                }).AddEntityFrameworkStores<ApplicationDBContext>();

            _ = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                    options.DefaultChallengeScheme =
                    options.DefaultForbidScheme =
                    options.DefaultScheme =
                    options.DefaultSignInScheme =
                    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"] ?? "")
                        )
                    };
                });

            _ = services.AddAuthorization();

            return services;
        }
    }
}
