using DotNetEnv;
using Hangfire;
using Hangfire.PostgreSql;

namespace PickleBall.Extension
{
    public static class CronJobExtension
    {
        public static IServiceCollection AddCronJob(this IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            Env.Load($".env.{env.ToLower()}");

            services.AddHangfire(config => config
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UsePostgreSqlStorage(Env.GetString("CONNECTION_STRING")));        

            services.AddHangfireServer();
            return services;
        }
    }
}
