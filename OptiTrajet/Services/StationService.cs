using Microsoft.EntityFrameworkCore;
using OptiTrajet.Dtos.In;
using OptiTrajet.Dtos.Out;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;

namespace OptiTrajet.Services
{
    public class StationService : IStationService
    {
        private readonly OptiTrajetContext _dbContext;

        private const decimal latconv = ;
        private const decimal lonconv = ;

        public StationService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StationDto>> Get(GetStations command)
        {
            var query = _dbContext.Stations.AsQueryable();

            if (command.Lines.Length > 0)
            {
                query = query.Where(x => command.Lines.Contains(x.LineId));
            }

            if (command.Radius > 0)
            {
                query.AsEnumerable().Where(x =>
                {
                    return command.Radius >= Math.Sqrt(Math.Pow((double)Math.Abs((command.Lat - x.Lat) * 111.0M), 2) + Math.Pow((double)Math.Abs((command.Lon - x.Lon) * 111.321M), 2));
                });
            }

            return await query.Select(x => new StationDto
            {
                Id = x.Id,
                Lat = x.Lat,
                Lon = x.Lon,
                Name = x.Name,
                Color = x.Line.Color
            }).ToListAsync();
        }
    }
}
