using Microsoft.EntityFrameworkCore;
using MVCandKAFKA3.Data;
using MVCandKAFKA3.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Database - PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kafka Services
builder.Services.AddSingleton<KafkaProducerService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<KafkaProducerService>>();
    return new KafkaProducerService(
        config["Kafka:BootstrapServers"],
        config["Kafka:Topic"],
        logger);
});

// Feedback Consumer (Background Service)
builder.Services.AddHostedService<FeedbackConsumerService>();

// Excel Service
builder.Services.AddScoped<ExcelService>();

var app = builder.Build();

// Configure the HTTP request pipeline
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
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();