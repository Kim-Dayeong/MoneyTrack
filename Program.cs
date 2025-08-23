using Microsoft.EntityFrameworkCore;
using MoneyTrack.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext 등록 (appsettings.json의 ConnectionStrings:Default 사용)
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs!, ServerVersion.AutoDetect(cs)));
      // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)); // 조회 위주면 편함

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // 필요하다면 ↓ (대소문자 구분 안 하고 싶을 때)
        // options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 2) ( DB  체크 엔드포인트: 접속 확인용
app.MapGet("/health/db", async (AppDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return Results.Json(new { ok });
});
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
