using DotNetEnv;
using Hangfire;
using Hangfire.PostgreSql;

namespace PickleBall.Extension
{
    public static class CronJobExtension
    {
        public static IServiceCollection AddCronJob(this IServiceCollection services)
        {
            Env.Load();
            services.AddHangfire(config => config
                 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                 .UseSimpleAssemblyNameTypeSerializer()
                 .UseRecommendedSerializerSettings()
                 .UsePostgreSqlStorage(Env.GetString("CONNECTION_STRING").ToString()));        

            services.AddHangfireServer();
            return services;
        }
    }
}
