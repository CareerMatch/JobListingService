using JoblistingService.Config;
using JoblistingService.Repositories;
using JoblistingService.Services;
using Prometheus;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// Register services and repositories
builder.Services.AddScoped<IJobListingRepository, JobListingRepository>();
builder.Services.AddScoped<IJobListingService, JobListingService>();

// Add Redis Caching
var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString"); // e.g., "localhost:6379"
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "JobListingService:";
});

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer(); // Default: /metrics
app.UseHttpMetrics(); 

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();