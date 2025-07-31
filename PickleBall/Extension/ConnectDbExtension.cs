using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PickleBall.Data;
using Serilog;

namespace PickleBall.Extension
{
    public static class ConnectDbExtension
    {
        public static IServiceCollection AddConnetion(this IServiceCollection services)
        {
            Env.Load();

            var connStr = $"Host={Env.GetString("DB_HOST")};Database={Env.GetString("DB_NAME")};Username={Env.GetString("DB_USER")};Password={Env.GetString("DB_PASSWORD")}";

            try
            {
                services.AddEntityFrameworkNpgsql().AddDbContext<BookingContext>(opt =>
                {
                    opt.UseNpgsql(connStr);
                });
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"Không thể kết nối tới database ${ex.Message}");
            }
            catch (Exception ex) {
                Log.Error($"Lỗi khác {ex.Message}");
            }
            

            return services;
        }
    }
}
