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
            services.AddValidatorsFromAssemblyContaining<TimeSlotRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CourtRequestValidation>();
            services.AddValidatorsFromAssemblyContaining<ForgetPasswordRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangePasswordRequestValidator>();
            return services;
        }
    }
}
