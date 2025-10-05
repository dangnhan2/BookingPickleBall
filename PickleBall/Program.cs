using Hangfire;
using Microsoft.OpenApi.Models;
using PickleBall.Data;
using PickleBall.Extension;
using PickleBall.Service.BackgoundJob;
using PickleBall.Service.SignalR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Extensions();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Pickle Ball", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("PickleBall");
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<BookingHub>("/bookingHub");

app.UseHangfireDashboard("/hangfire");


RecurringJob.AddOrUpdate<IBackgroundJob>(
           "DeleteExpiredRefreshToken",
           service => service.DeleteExpiredRefreshToken(),
           "0 3 * * *"
);

RecurringJob.AddOrUpdate<IBackgroundJob>(
    "CheckAndReleaseExpiredBookings",
    service => service.CheckAndReleaseExpiredBookings(),
    "*/1 * * * *"  // chạy mỗi phút
);

app.MapControllers();

app.Run();

