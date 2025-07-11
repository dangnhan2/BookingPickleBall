﻿namespace PickleBall.Extension
{
    public static  class CorsExtension
    {
        public static IServiceCollection AddCors(this IServiceCollection services) {
            services.AddCors(options =>
            {
                options.AddPolicy("PickleBall", policy =>
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                );
            });
            return services;
        }
    }
}
