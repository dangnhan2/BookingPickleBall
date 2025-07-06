namespace PickleBall.Extension
{
    public static class ApplicationExtension
    {
        public static IServiceCollection Extensions(this IServiceCollection services) {

            services.AddConnetion();
            services.AddJwt();
            return services;
        }
    }
}
