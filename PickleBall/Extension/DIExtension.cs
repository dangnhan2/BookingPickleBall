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
          services.AddScoped<IUserRepo, UserRepo>();
          services.AddScoped<IBookingRepo, BookingRepo>();
          services.AddScoped<IPaymentRepo, PaymentRepo>();
          services.AddScoped<IBookingTimeSlotRepo, BookingTimeSlotRepo>();
            
          services.AddScoped<IJwtService, JwtService>();    
          services.AddScoped<IAccountService, AccountService>();
          services.AddScoped<ICourtService, CourtService>();
          services.AddScoped<ITimeSlotService, TimeSlotService>();
          services.AddScoped<IBlogService, BlogService>();
          services.AddScoped<IUserService, UserService>();
          services.AddTransient<IEmailService, EmailService>();
          services.AddTransient<ICloudinaryService, CloudinaryService>();
          services.AddTransient<ICheckoutService, CheckoutService>();
          services.AddScoped<IBookingService, BookingService>();
          services.AddScoped<ICronJobService, CronJobService>();
          services.AddScoped<IPayOsWebHookService, PayOsWebHookService>();
          return services;
        }
    }
}
