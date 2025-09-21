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
            try
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                Env.Load($".env.{env.ToLower()}");

                var connStr = Env.GetString("CONNECTION_STRING");

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
