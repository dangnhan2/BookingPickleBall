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
            services.AddControllers().AddJsonOptions(ops =>
            {
                ops.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            });
         
            return services;
        }
    }
}
