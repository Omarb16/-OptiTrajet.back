using Microsoft.EntityFrameworkCore;
using OptiTrajet.Domain.Entities;
using OptiTrajet.Dtos;
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

        public async Task<List<Station>> Get()
        {
            return await _dbContext.Stations.ToListAsync();
        }

        public async Task<List<StationDto>> GetStationsDto(GetStationsDto query)
        {
            return await _dbContext.Itineraries
            .Where(x => x.Duration <= query.Duration && x.PlaceId == query.PlaceId)
            .Select(x => new StationDto
            {
                Id = x.Id,
                Lat = x.Station.Lat,
                Lon = x.Station.Lon,
                Name = x.Station.Name,
                CityId = x.Station.CityId,
                Color = x.Station.Line.Color,
                Duration = x.Duration
            }).ToListAsync();
        }
    }

    public record GetStationsDto
    {
        public Guid PlaceId { get; set; }
        public int Duration { get; set; } = 0;
    }
}
