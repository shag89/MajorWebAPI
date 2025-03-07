using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Core.DataAccess;
using WebAPI.Core.EntitiManagmentService;
using WebAPI.Core.Model;
using WebAPI.Core.Service;
using WebAPI.DataAccess;
using WebAPI.Services;
using WebAPI.Services.Services;

namespace MajorWebAPI.Extensions
{
    public static class ConfigureService
    {
        private static IConfiguration _Configuration;

        public static void Configrue(IServiceCollection services,IConfiguration configuration)
        {
            _Configuration = configuration;

            services.AddCors();
            services.InjectDependancies();
            services.AddJWTAuthConfigurations();
            services.AddControllers();

        }
        public static IServiceCollection InjectDependancies(this IServiceCollection services)
        {
            services.AddSingleton<DapperContext>(ctx => new DapperContext(_Configuration.GetConnectionString("SqlConnectionString")));
            //services.AddScoped<typeof(IDataAccessService<>), typeof(DataAccessService<>)> (ctx => new DataAccessService(_Configuration.GetConnectionString("SqlConnectionString")));
            services.AddScoped<IDataAccessLayer, DataAccessLayer>(ctx =>
            {
                var dbcontext = new DapperContext(_Configuration.GetConnectionString("SqlConnectionString"));
                return new DataAccessLayer(dbcontext);
            });
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IWebApiResponceService, WebAPICommonResponse>();
            
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserManagementService, UserManagementService>();


            return services;
        }
        public static IServiceCollection AddJWTAuthConfigurations(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _Configuration["JWT:Issuer"],
                    ValidAudience = _Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // We can add policies with using claims like below to authorization. to Activate this policy need to add in controller level
            services.AddAuthorization(options =>
            {
                options.AddPolicy("PolicyOne", policy =>
                {
                    policy.RequireRole("User")
                    .RequireClaim("Designation", "SE");
                });

                // We can add custom Authorization requirements
                options.AddPolicy("AgeRestriction", policy =>
                {
                    policy.Requirements.Add(new AgeRestrictionRequirement(21));
                });
            });
            // Need to registers the handler so that ASP.NET Core can inject it when processing authorization for the policy
            services.AddSingleton<IAuthorizationHandler, AgeRestrictionHndler>();

            return services;
        }
    }
}
