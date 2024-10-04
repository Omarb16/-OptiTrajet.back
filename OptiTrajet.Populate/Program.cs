using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptiTrajet.Persistence;
using OptiTrajet.Populate.Services;
using Serilog;
using Serilog.Events;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true).Build();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

var app = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddScoped<IPopulateDb, PopulateDb>();
        services.AddDbContext<OptiTrajetContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
    })
    .UseSerilog(Log.Logger)
    .Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetService<IPopulateDb>()!;
    await service.Populate();
    // https://france-geojson.gregoiredavid.fr/
    //service.ModifiedCities();
    // https://data.iledefrance-mobilites.fr/explore/dataset/emplacement-des-gares-idf/export/
    //service.ModifiedStations();
    //service.Count();
}

