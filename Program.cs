using System.Text.Encodings.Web;
using System.Text.Unicode;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using test2.Models;
using test2.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});
//memory service
builder.Services.AddDistributedMemoryCache();

#region 逾期預約排程
//內建版
//builder.Services.AddHostedService<ReservationService>();

// Hangfire版
//builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("Test2ConnString")));
//builder.Services.AddHangfireServer();
//RecurringJob.AddOrUpdate<ReservationService>
//    ("我是排程"
//    , service => service.ExecuteAsync()
//    , Cron.Minutely);
#endregion

//session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// 我是DB Start
builder.Services.AddSession();
builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("Test2ConnString");
builder.Services.AddDbContext<Test2Context>(x => x.UseSqlServer(connectionString));
// DB END

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// HandFire頁面
//app.UseHangfireDashboard();


//session start
app.UseSession();

//fronted area
//app.MapControllerRoute(
//    name: "FrontendArea",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
//    defaults: new { area = "Frontend" }
//);

//backed area
app.MapControllerRoute(
    name: "BackendArea",
    pattern: "{area:exists}/{controller=Manage}/{action=Index}/{id?}",
    defaults: new { area = "Backend" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();