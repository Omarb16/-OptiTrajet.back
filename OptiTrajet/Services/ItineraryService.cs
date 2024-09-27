using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OptiTrajet.Domain.Entities;
using OptiTrajet.Domain.In;
using OptiTrajet.Domain.States;
using OptiTrajet.Exceptions;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;
using Spire.Xls;
using System.Data;
using System.Net.Http.Headers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OptiTrajet.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly OptiTrajetContext _dbContext;

        private const string API_KEY = "cb90a96a-85b7-4c80-9798-b774afbc3c7b";
        private const string URI = "https://api.navitia.io";

        public ItineraryService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task FindOptimalCommute(FindOptimalCommute command)
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

            var client = CreacteCLient();
            var fTime = GetTime();

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

                foreach (var st in stations[i])
                {
                    var itinerary = new Itinerary
                    {
                        StationId = st.Id,
                        PlaceId = place.Id,
                        Duration = duration / 60,
                    };

                    await _dbContext.Itineraries.AddAsync(itinerary);
                    await _dbContext.SaveChangesAsync();
                }
            }

            client.Dispose();
        }

        public async Task<MemoryStream> GetReport(GetReport command)
        {
            var place = await _dbContext.Places.Where(x => x.Lat == Math.Round(command.Lat, 3) && x.Lon == Math.Round(command.Lon, 3)).FirstOrDefaultAsync();

            if (place == null)
            {
                throw new FunctionalException("Find optimal commute routes first");
            }

            var cities = await _dbContext.Itineraries.Where(x => x.PlaceId == place.Id && x.Duration <= command.Duration)
                .Select(x => new CityEntity
                {
                    Name = x.Station.City.Name,
                    CodePostal = x.Station.City.CodePostal
                }).ToArrayAsync();

            cities = cities.DistinctBy(x => x.Name).ToArray();

            Workbook workbook = new Workbook();

            workbook.Worksheets.Clear();
            Worksheet worksheet = workbook.Worksheets.Add("0");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("City", typeof(string));
            dataTable.Columns.Add("Code Postal", typeof(string));

            CellStyle fontStyle = workbook.Styles.Add("header");

            fontStyle.Font.Size = 11;
            fontStyle.Font.FontName = "Calibri";

            worksheet.ApplyStyle(fontStyle);

            for (int i = 0; i < cities.Length; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr[0] = cities[i].Name;
                dr[1] = cities[i].CodePostal;
                dataTable.Rows.Add(dr);
            }

            worksheet.InsertDataTable(dataTable, true, 1, 1, true);

            MemoryStream stream = new MemoryStream();
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
            int daysToAdd = (1 - (int)DateTime.UtcNow.DayOfWeek + 7) % 7;
            var time = DateTime.UtcNow.AddDays(daysToAdd);
            time = time.AddHours(7 - time.Hour);
            time = time.AddMinutes(30 - time.Minute);
            time = time.AddSeconds(0 - time.Second);

            return time.ToString("yyyyMMddTHHmmss");
        }

        private static HttpClient CreacteCLient()
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
