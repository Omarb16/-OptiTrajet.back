using Microsoft.EntityFrameworkCore;
using OptiTrajet.Persistence;
using OptiTrajet.Services;
using OptiTrajet.Services.Interfaces;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration["applicationUrl"])
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true);
    });
});


Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", path: @"./logs/OptiTrajet-.txt", rollingInterval: RollingInterval.Month)
            .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddDbContext<OptiTrajetContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<IItineraryService, ItineraryService>();
builder.Services.AddTransient<IStationService, StationService>();
builder.Services.AddTransient<ILineService, LineService>();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Log.Logger.Information("Sever starting");

app.Run();