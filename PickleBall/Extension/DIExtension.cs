using PickleBall.Repository.Blogs;
using PickleBall.Repository.Bookings;
using PickleBall.Repository.Courts;
using PickleBall.Repository.Other;
using PickleBall.Repository.TimeSlots;
using PickleBall.Repository.Users;
using PickleBall.Service.Auth;
using PickleBall.Service.BackgoundJob;
using PickleBall.Service.Blogs;
using PickleBall.Service.Bookings;
using PickleBall.Service.Checkout;
using PickleBall.Service.Courts;
using PickleBall.Service.Email;
using PickleBall.Service.SoftService;
using PickleBall.Service.Storage;
using PickleBall.Service.TimeSlots;
using PickleBall.Service.Users;
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
          services.AddScoped<IBackgroundJob, BackgroundJob>();
          services.AddScoped<IPayOSService, PayOSService>();
          return services;
        }
    }
}
