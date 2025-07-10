using PickleBall.Repository;
using PickleBall.Service;
using PickleBall.Service.SoftService;
using PickleBall.UnitOfWork;

namespace PickleBall.Extension
{
    public static class DIExtension
    {
        public static IServiceCollection AddDI(this IServiceCollection services) { 
          
          services.AddScoped<ICourtRepo, CourtRepo>();
          services.AddScoped<ITimeSlotRepo, TimeSlotRepo>();
          services.AddScoped<IBlogRepo, BlogRepo>();
          services.AddScoped<IUnitOfWorks, UnitOfWorks>();
            
          services.AddScoped<IJwtService, JwtService>();    
          services.AddScoped<IAccountService, AccountService>();
          services.AddScoped<ICourtService, CourtService>();
          services.AddScoped<ITimeSlotService, TimeSlotService>();
          services.AddScoped<IBlogService, BlogService>();
          services.AddTransient<IEmailService, EmailService>();
          services.AddTransient<ICloudinaryService, CloudinaryService>();
          return services;
        }
    }
}
