using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PickleBall.Data;
using PickleBall.Models;
using System.Text;

namespace PickleBall.Extension
{
    public static class JwtExtension
    {
        public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            Env.Load();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+ ";
            });

            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>() // add roles
                .AddRoleManager<RoleManager<IdentityRole>>() //  make use of RoleManager
                .AddUserManager<UserManager<User>>() // make use of UserManager to create user
                .AddSignInManager<SignInManager<User>>() // make user of Sign in manager
                .AddEntityFrameworkStores<BookingContext>()
                .AddDefaultTokenProviders();

            // Add authentication
            services
                .AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                 // add bearer
                 .AddJwtBearer(option =>
                 {
                     option.SaveToken = true;
                     option.RequireHttpsMetadata = false;
                     option.TokenValidationParameters = new TokenValidationParameters()
                     {
                         //validate the token based on the key we have provided inside the appsettings.json JWT:Secret
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = Env.GetString("ISSUER"),
                         ValidAudience = Env.GetString("AUDIENCE"),
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET_KEY")))
                     };
                 });
            return services;
        }
    }
}
