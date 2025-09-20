using Hangfire;

namespace PickleBall.Extension
{
    public static class ApplicationExtension
    {
        public static IServiceCollection Extensions(this IServiceCollection services) {

            services.AddConnetion();
            services.AddJwt();
            services.AddDI();
            services.AddCors();
            services.AddCronJob();
            services.AddSignalR();

            return services;
        }
    }
}
