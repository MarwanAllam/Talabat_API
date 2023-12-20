 using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Talabat.Apis.Extentions;
using Talabat.Apis.MiddleWares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.APIs
{
    public class Program
    {
        // Entry Point
        public static async Task Main(string[] args)
        {

            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.

            webApplicationBuilder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            });

            webApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });

            webApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = webApplicationBuilder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
             
            //webApplicationBuilder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            //webApplicationBuilder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            //webApplicationBuilder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();

            webApplicationBuilder.Services.AddApplicationServices();

            webApplicationBuilder.Services.AddIdentityServices(webApplicationBuilder.Configuration);

            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins(webApplicationBuilder.Configuration["FrontBaseUrl"]);
                });
            });



            //webApplicationBuilder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));

            //webApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>(option =>
            //{

            //    //option.Password.RequiredUniqueChars = 2;
            //    //option.Password.RequireNonAlphanumeric = true;
            //    //option.Password.RequireDigit = true;
            //    //option.Password.RequireUppercase = true;
            //    //option.Password.RequireLowercase = true;

            //}).AddEntityFrameworkStores<AppIdentityDbContext>();

            #endregion

            var app = webApplicationBuilder.Build();

            #region Update-Database

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<StoreContext>();

            var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();

           var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);

                await _identityDbContext.Database.MigrateAsync();

                var _userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(_userManager);
            }
            catch (Exception ex)
            {


                
                logger.LogError(ex, "an Error has been occured during apply the migration");

            }

            #endregion

            #region Configure Kestrel Middleares

            app.UseMiddleware<ExceptionMiddleWare>();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithRedirects("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.MapControllers();

            app.UseAuthentication();

            app.UseAuthorization();




            #endregion

            app.Run();
        }
    }
}