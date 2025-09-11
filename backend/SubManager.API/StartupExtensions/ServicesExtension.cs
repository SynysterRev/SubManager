using JuniorOnly.Application.Commons;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SubManager.Application.Interfaces;
using SubManager.Application.Services;
using SubManager.Domain.IdentityEntities;
using SubManager.Domain.Repositories;
using SubManager.Infrastructure.DatabaseContext;
using SubManager.Infrastructure.Repositories;
using System.Text;

namespace SubManager.API.StartupExtensions
{
    public static class ServicesExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IJwtService, JwtService>();

            services.Configure<PaginationOptions>(configuration.GetSection(PaginationOptions.SectionName));

            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            var defaultOrigins = new string[] { "http://localhost:4200" };

            // CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .WithOrigins(allowedOrigins ?? defaultOrigins)
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
                });
            });

            string? audience = configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(audience))
            {

                return;
            }

            string? issuer = configuration["Jwt:Issuer"];

            if (string.IsNullOrEmpty(issuer))
            {

                return;
            }

            string? key = configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {

                return;
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };
            });
        }
    }
}
