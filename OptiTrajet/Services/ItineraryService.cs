using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OptiTrajet.Domain.Entities;
using OptiTrajet.Dtos;
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

        private const string API_KEY = "cb90a96a-85b7-4c80-9798-b774afbc3c7b";
        private const string URI = "https://api.navitia.io";

        public ItineraryService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task FindOptimalCommute(FindOptimalCommute command)
        {
            var client = CreacteCLient();
            var fTime = GetTime();

            var place = await _dbContext.Places.Where(x => x.Id == command.PlaceId).FirstOrDefaultAsync();

            if(place == null)
            {
                throw new Exception("Place not found");
            }

            var stationsIdWithItinerary = await _dbContext.Itineraries.Where(x => x.PlaceId == command.PlaceId).Select(x => x.StationId).ToListAsync();
            List<Guid> stationIds = await _dbContext.Stations.Select(x => x.Id).ToListAsync();
            var stationsIdWithoutItinerary = stationIds.Except(stationsIdWithItinerary).ToList();

            var stations = await _dbContext.Stations.Where(b => stationsIdWithoutItinerary.Contains(b.Id)).Select(s => new StationLatLonDto
            {
                Id = s.Id,
                Lat = s.Lat,
                Lon = s.Lon,

            }).GroupBy(g => g.Lat.ToString() + g.Lon.ToString()).ToListAsync();

            foreach (var s in stations)
            {
                var station = s.First();

                var duration = -1;

                var res = await GetJourney(client, station, place.Lat, place.Lon, fTime);

                if (res?.Journeys?.Count > 0)
                {
                    duration = res.Journeys.Select(s => s.duration).Min();
                }

                foreach (var st in s)
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

        private async Task<ByteArrayContent> GenerateReport(Guid placeId)
        {
            var cities = await _dbContext.Itineraries.Where(x => x.Id == placeId).Select(x => new Report
            {
                Name = x.Station.City.Name,
                CodePostal = x.Station.City.CodePostal
            }).ToListAsync();

            return CreateExcel(cities);
        }

        private static async Task<JourneyList> GetJourney(HttpClient client, StationLatLonDto station, decimal lat, decimal lon, string time)
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

        private ByteArrayContent CreateExcel(List<Report> cities)
        {
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

            cities.ForEach(s =>
            {
                DataRow dr = dataTable.NewRow();
                dr[0] = s.Name;
                dr[0] = s.CodePostal;
                dataTable.Rows.Add(dr);
            });

            worksheet.InsertDataTable(dataTable, true, 1, 1, true);

            var name = "Report_" + DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".xlsx";

            var stream = new MemoryStream();
            workbook.SaveToStream(stream, FileFormat.Version2016);

            return new ByteArrayContent(stream.ToArray());
        }
    }

    public record FindOptimalCommute
    {
        public Guid PlaceId { get; set; }
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
