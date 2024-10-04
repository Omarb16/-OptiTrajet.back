using Microsoft.EntityFrameworkCore;
using OptiTrajet.Domain.Out;
using OptiTrajet.Persistence;
using OptiTrajet.Services.Interfaces;

namespace OptiTrajet.Services
{
    public class LineService : ILineService
    {
        private readonly OptiTrajetContext _dbContext;

        public LineService(OptiTrajetContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LineDto[]> Get()
        {
            return await _dbContext.Lines.Select(s => new LineDto
            {
                Id = s.Id,
                Name = s.Name,
            }).OrderBy(x => x.Name).ToArrayAsync();
        }
    }
}
