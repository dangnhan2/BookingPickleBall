using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Validation;

namespace PickleBall.Extension
{
    public static class ValidationExtension
    {
        public static IServiceCollection AddValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<TimeSlotRequestValidation>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidation>();
            services.AddValidatorsFromAssemblyContaining<CourtRequestValidation>();
            return services;
        }
    }
}
