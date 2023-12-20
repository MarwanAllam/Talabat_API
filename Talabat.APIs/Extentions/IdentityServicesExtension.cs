using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.Apis.Extentions
{
    public static class IdentityServicesExtension
    {

        public static IServiceCollection AddIdentityServices(this IServiceCollection Services ,IConfiguration configuration )
        {
            Services.AddScoped(typeof(IAuthService), typeof(AuthService));

            Services.AddIdentity<AppUser, IdentityRole>(option =>
            {

                //option.Password.RequiredUniqueChars = 2;
                //option.Password.RequireNonAlphanumeric = true;
                //option.Password.RequireDigit = true;
                //option.Password.RequireUppercase = true;
                //option.Password.RequireLowercase = true;

            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            Services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/ options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:ValidAudience"],
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromDays(double.Parse(configuration["JWT:DurationInDays"]))
                    };
                });



            return Services;

        }



    }
}
