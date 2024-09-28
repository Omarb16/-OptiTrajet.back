using Microsoft.EntityFrameworkCore;
using OptiTrajet.Domain.In;
using OptiTrajet.Domain.Out;
using OptiTrajet.Domain.States;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;

namespace OptiTrajet.Services
{
    public class StationService : IStationService
    {
        private readonly OptiTrajetContext _dbContext;

        public StationService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StationDto[]> Get(GetStations command)
        {
            var query = _dbContext.Stations.AsQueryable();

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

            var stations = await query.Select(x => new StationDto
            {
                Id = x.Id,
                Lat = x.Lat,
                Lon = x.Lon,
                Name = x.Name,
                Color = x.Line.Color,
                Duration = -1
            }).ToArrayAsync();

            var place = await _dbContext.Places.Where(x => x.Lat == Math.Round(command.Lat, 3) && x.Lon == Math.Round(command.Lon, 3)).FirstOrDefaultAsync();

            if (place == null)
            {
                return stations;
            }

            for (int i = 0; i < stations.Length; i++)
            {
                var d = await _dbContext.Itineraries.Where(x => x.PlaceId == place.Id && x.StationId == stations[i].Id).Select(x => new { x.Duration }).FirstOrDefaultAsync();

                if (d != null)
                {
                    stations[i].Duration = d.Duration;
                }
            }

            return stations;
        }
    }
}
