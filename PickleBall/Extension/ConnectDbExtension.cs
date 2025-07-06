using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using PickleBall.Data;

namespace PickleBall.Extension
{
    public static class ConnectDbExtension
    {
        public static IServiceCollection AddConnetion(this IServiceCollection services)
        {
            Env.Load();

            var connStr = $"Host={Env.GetString("DB_HOST")};Database={Env.GetString("DB_NAME")};Username={Env.GetString("DB_USER")};Password={Env.GetString("DB_PASSWORD")}";

            services.AddEntityFrameworkNpgsql().AddDbContext<BookingContext>(opt =>
            {
                opt.UseNpgsql(connStr);
            });

            return services;
        }
    }
}
