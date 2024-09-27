using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OptiTrajet.Domain.Out;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;

namespace OptiTrajet.Services
{
    public class CityService : ICityService
    {
        private readonly OptiTrajetContext _dbContext;

        public CityService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CityDto[]> Get()
        {
            return await _dbContext.Cities.Select(s => new CityDto
            {
                Id = s.Id,
                Name = s.Name,
                Coordianates = JsonConvert.DeserializeObject<decimal[][]>(s.Coordianates)!,
            }).ToArrayAsync();
        }
    }
}
