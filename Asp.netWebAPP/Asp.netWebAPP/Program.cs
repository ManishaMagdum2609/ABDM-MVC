using Asp.netWebAPP.Core.Application.ABHA.Commands.Handlers;
using Asp.netWebAPP.Core.Application.ABHA.Queries.Handler;
using Asp.netWebAPP.Core.Application.Interface;
using Asp.netWebAPP.Infrastructure.Data;
using Asp.netWebAPP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Database contexts
builder.Services.AddDbContext<AbdmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<DanpheDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DanpheDb")));

// Services
builder.Services.AddScoped<IAbhaLoginService, Abhalogin_Service>();

// Handlers
builder.Services.AddScoped<SearchAbhaHandler>();
builder.Services.AddScoped<RequestOtpLoginHandler>();
builder.Services.AddScoped<VerifyOtpHandler>();
builder.Services.AddScoped<SearchPatientByMobileHandler>();

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
