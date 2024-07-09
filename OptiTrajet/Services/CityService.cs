using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OptiTrajet.Dtos;
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

        public async Task<List<CityDto>> Get()
        {
            return await _dbContext.Cities.Select(s => new CityDto
            {
                Id = s.Id,
                Name = s.Name,
                Coordianates = JsonConvert.DeserializeObject<decimal[][]>(s.Coordianates)!,
            }).ToListAsync();
        }
    }
}
