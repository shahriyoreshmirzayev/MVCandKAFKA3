using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVCandKAFKA3.Data;
using MVCandKAFKA3.Services;
using MVCandKAFKA3.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. appsettings.json dan ConnectionString va Kafka sozlamalarini o‘qish
var configuration = builder.Configuration;

// 2. PostgreSQL (haqiqiy baza, Dockerda emas)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// 3. Kafka Producer – konfiguratsiyani appsettings.json dan olamiz
builder.Services.AddSingleton<KafkaProducerService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<KafkaProducerService>>();

    var bootstrapServers = configuration["Kafka:BootstrapServers"];
    var topic = configuration["Kafka:Topic"];

    return new KafkaProducerService(bootstrapServers, topic, logger);
});

// 4. Excel Service
builder.Services.AddScoped<ExcelService>();

// 5. MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 6. Bazani avtomatik yaratish va test ma'lumotlarini yuklash
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Migration bo‘lsa ishlaydi, yo‘q bo‘lsa oddiy Create
    // Agar migration yo‘q bo‘lsa, EnsureCreated ishlatish mumkin:
    // context.Database.EnsureCreated();
}

// 7. HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();