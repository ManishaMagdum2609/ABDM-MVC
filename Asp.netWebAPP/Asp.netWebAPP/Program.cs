using Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers;
using Asp.netWebAPP.Core.Application.ABHA.Queries.Handler;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Core.Application.M2.Handlers;
using Asp.netWebAPP.Infrastructure.Data;
using Asp.netWebAPP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddHttpClient();
builder.Services.AddDataProtection();
// Database contexts
builder.Services.AddDbContext<AbdmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDbContext<DanpheDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DanpheDb")));
builder.Services.AddDbContext<DanpheDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DanpheDb"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));


// Services
builder.Services.AddScoped<IAbhaLoginService, Abhalogin_Service>();
builder.Services.AddScoped<IAbhaRegistrationService,AbhaRegistrationService>();
builder.Services.AddScoped<IAbhaAuthService,AbhaAuthService>();
builder.Services.AddScoped<IBridgeService, BridgeService>();
builder.Services.AddScoped<ICareContextLinkService, CareContextLinkService>();



// Handlers
builder.Services.AddScoped<SearchAbhaHandler>();
builder.Services.AddScoped<RequestOtpLoginHandler>();
builder.Services.AddScoped<VerifyOtpHandler>();
builder.Services.AddScoped<SearchPatientByMobileHandler>();
builder.Services.AddScoped<RequestRegisterOtpHandler>();
builder.Services.AddScoped<VerifyRegisterOtpHandler>();
builder.Services.AddScoped<LinkCareContextHandler>();
builder.Services.AddScoped<RegisterBridgeServiceHandler>();

// Enable CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Apply CORS
app.UseCors("AllowAngularDev");

app.UseAuthorization();

// Map API routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
