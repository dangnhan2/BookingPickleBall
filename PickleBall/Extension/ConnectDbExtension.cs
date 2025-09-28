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
                Log.Information($"Environment: {env}");

                // Thử load file .env theo environment
                var envFile = $".env.{env.ToLower()}";
                Log.Information($"Trying to load: {envFile}");

                if (File.Exists(envFile))
                {
                    Env.Load(envFile);
                    Log.Information($"Loaded {envFile} successfully");
                }
                else
                {
                    // Thử load file .env mặc định
                    if (File.Exists(".env"))
                    {
                        Env.Load(".env");
                        Log.Information("Loaded .env successfully");
                    }
                    else
                    {
                        Log.Warning("No .env file found, trying to read from appsettings");
                    }
                }

                var connStr = Env.GetString("CONNECTION_STRING");

                // Nếu không đọc được từ .env, thử đọc từ appsettings
                if (string.IsNullOrEmpty(connStr))
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{env}.json", optional: true)
                        .Build();

                    connStr = configuration.GetConnectionString("DefaultConnection");
                    Log.Information("Using connection string from appsettings");
                }

                if (string.IsNullOrEmpty(connStr))
                {
                    throw new Exception("Connection string not found in .env or appsettings");
                }

                Log.Information($"Connection string found: {connStr.Substring(0, Math.Min(50, connStr.Length))}...");

                services.AddEntityFrameworkNpgsql().AddDbContext<BookingContext>(opt =>
                {
                    opt.UseNpgsql(connStr);
                });

                Log.Information("Database context configured successfully");
            }
            catch (NpgsqlException ex)
            {
                Log.Error($"Không thể kết nối tới database: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi khác: {ex.Message}");
                Log.Error($"Stack trace: {ex.StackTrace}");
            }

            return services;
        }
    }
}
