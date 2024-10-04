using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OptiTrajet.Domain.Entities;
using OptiTrajet.Domain.In;
using OptiTrajet.Domain.States;
using OptiTrajet.Exceptions;
using OptiTrajet.Hubs;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;
using Spire.Xls;
using System.Data;
using System.Net.Http.Headers;

namespace OptiTrajet.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly OptiTrajetContext _dbContext;
        private readonly Serilog.ILogger _logger;
        private readonly ISocketHub _socket;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string API_KEY;

        private const string URI = "https://api.navitia.io";

        public ItineraryService(OptiTrajetContext dbContext, ISocketHub socket, IConfiguration configuration, Serilog.ILogger logger, IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _socket = socket;
            _scopeFactory = scopeFactory;

            API_KEY = configuration["API_KEY"]?.ToString();

            if (API_KEY == null)
            {
                throw new FunctionalException("API KEY missing");
            }
        }

        public async Task FindItineraries(FindOptimalCommute command)
        {
            var place = await _dbContext.Places.Where(x => x.Lat == Math.Round(command.Lat, 3) && x.Lon == Math.Round(command.Lon, 3)).FirstOrDefaultAsync();

            if (place == null)
            {
                place = new Place
                {
                    Lat = Math.Round(command.Lat, 3),
                    Lon = Math.Round(command.Lon, 3),
                };

                await _dbContext.Places.AddAsync(place);

                await _dbContext.SaveChangesAsync();
            }

            var stationsIdWithItinerary = await _dbContext.Itineraries.Where(x => x.PlaceId == place.Id).Select(x => x.StationId).Distinct().ToArrayAsync();

            var query = _dbContext.Stations.Where(b => !stationsIdWithItinerary.Contains(b.Id)).AsQueryable();

            if (command.Lines.Length > 0)
            {
                query = query.Where(x => command.Lines.Contains(x.LineId));
            }
            if (command.Radius > 0)
            {
                query = query.Where(x =>
                command.Radius >= 2 * 6371 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin(((double)x.Lat * Math.PI / 180 - (double)command.Lat * Math.PI / 180) / 2), 2) +
                        Math.Cos((double)command.Lat * Math.PI / 180) *
                        Math.Cos((double)x.Lat * Math.PI / 180) *
                        Math.Pow(Math.Sin(((double)x.Lon * Math.PI / 180 - (double)command.Lon * Math.PI / 180) / 2), 2)))
                );
            }

            var stations = await query.Select(s => new StationLatLon
            {
                Id = s.Id,
                Lat = s.Lat,
                Lon = s.Lon,
            }).GroupBy(g => g.Lat.ToString() + "-" + g.Lon.ToString()).ToArrayAsync();

            if (stations.Length == 0)
            {
                throw new FunctionalException("Trajets déjà calculés");
            }

            SetItineraries(stations, place, command.ConnectionId);
        }

        public async Task SetItineraries(IGrouping<string, StationLatLon>[] stations, Place place, string connectionId)
        {
            try
            {
                await _socket.SendProgress(connectionId, 0);

                var client = CreacteClient();
                var fTime = GetTime();
                int lastProgress = 0;

                var itineraries = new List<Itinerary>(stations.Length);


                for (int i = 0; i < stations.Length; i++)
                {
                    var station = stations[i].FirstOrDefault();

                    if (station is null)
                    {
                        continue;
                    }

                    var duration = -1;

                    var res = await GetJourney(client, station, place.Lat, place.Lon, fTime);

                    if (res?.Journeys?.Count > 0)
                    {
                        duration = res.Journeys.Select(s => s.duration).Min();
                    }

                    var stationsLatLon = stations[i].ToArray();

                    for (int j = 0; j < stationsLatLon.Length; j++)
                    {
                        itineraries.Add(new Itinerary
                        {
                            StationId = stationsLatLon[j].Id,
                            PlaceId = place.Id,
                            Duration = duration / 60,
                        });
                    }

                    var progress = (int)Math.Floor((double)i / (double)stations.Length * 100);

                    if (progress >= lastProgress + 5)
                    {
                        await _socket.SendProgress(connectionId, progress);
                        lastProgress = progress;
                    }
                }

                client.Dispose();

                using (var scope = _scopeFactory.CreateScope())
                {
                    var _scopedDbContext = scope.ServiceProvider.GetRequiredService<OptiTrajetContext>();

                    await _scopedDbContext.Itineraries.AddRangeAsync(itineraries);
                    await _scopedDbContext.SaveChangesAsync();
                }

                await _socket.SendProgress(connectionId, 100);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ex}");
            }
        }

        public async Task<MemoryStream> GetReport(GetReport command)
        {
            var place = await _dbContext.Places.Where(x => x.Lat == Math.Round(command.Lat, 3) && x.Lon == Math.Round(command.Lon, 3)).FirstOrDefaultAsync();

            if (place == null)
            {
                throw new FunctionalException("Find optimal commute routes first");
            }

            var groupedReports = (await _dbContext.Itineraries.Where(x => x.PlaceId == place.Id && x.Duration <= command.Duration)
                .Select(x => new ReportEntity
                {
                    City = x.Station.City.Name,
                    CodePostal = x.Station.City.CodePostal,
                    Station = x.Station.Name,
                    Line = x.Station.Line.Name,
                    Duration = x.Duration
                })
                .GroupBy(x => x.City).ToArrayAsync())
                .Select(x => x.OrderBy(x => x.Duration).ToArray())
                .ToArray();

            var workbook = new Workbook();

            workbook.Worksheets.Clear();
            Worksheet worksheet = workbook.Worksheets.Add("0");

            CellStyle fontStyle = workbook.Styles.Add("header");

            fontStyle.Font.Size = 11;
            fontStyle.Font.FontName = "Calibri";

            worksheet.ApplyStyle(fontStyle);

            var dataTable = new DataTable();

            dataTable.Columns.Add("Ville", typeof(string));
            dataTable.Columns.Add("Code Postal", typeof(string));
            dataTable.Columns.Add("Station", typeof(string));
            dataTable.Columns.Add("Ligne", typeof(string));
            dataTable.Columns.Add("Durée", typeof(string));

            for (int i = 0; i < groupedReports.Length; i++)
            {
                var lines = groupedReports[i].ToArray();

                DataRow dr = dataTable.NewRow();
                dr[0] = lines[0].City;
                dr[1] = lines[0].CodePostal;
                dr[2] = lines[0].Station;
                dr[3] = lines[0].Line;
                dr[4] = lines[0].Duration.ToString() + " min";
                dataTable.Rows.Add(dr);

                for (int j = 1; j < lines.Length; j++)
                {
                    dr = dataTable.NewRow();
                    dr[2] = lines[j].Station;
                    dr[3] = lines[j].Line;
                    dr[4] = lines[j].Duration + " min";
                    dataTable.Rows.Add(dr);
                }
            }

            worksheet.InsertDataTable(dataTable, true, 1, 1, true);

            var stream = new MemoryStream();
            workbook.SaveToStream(stream, FileFormat.Version2016);
            stream.Position = 0;

            return stream;
        }

        private static async Task<JourneyList> GetJourney(HttpClient client, StationLatLon station, decimal lat, decimal lon, string time)
        {
            var response = await client.GetAsync($"v1/coverage/fr-idf/journeys?from={station.Lon};{station.Lat}&to={lon};{lat}&datetime={time}");

            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<JourneyList>(data)!;
        }

        private static string GetTime()
        {
            // Lundi 8h
            int daysToAdd = (1 - (int)DateTime.UtcNow.DayOfWeek + 7) % 7;
            var time = DateTime.UtcNow.AddDays(daysToAdd);
            time = time.AddHours(7 - time.Hour);
            time = time.AddMinutes(30 - time.Minute);
            time = time.AddSeconds(0 - time.Second);

            return time.ToString("yyyyMMddTHHmmss");
        }

        private HttpClient CreacteClient()
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri(URI),
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(API_KEY);
            return client;
        }
    }

    public record Report
    {
        public string Name { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
    }

    public class JourneyList
    {
        public List<Journey> Journeys { get; set; } = new List<Journey>(0);
    }

    public class Journey
    {
        public int duration { get; set; }
    }
}
