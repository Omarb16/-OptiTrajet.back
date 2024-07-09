using OptiTrajet.Domain.Entities;
using OptiTrajet.Dtos;

namespace OptiTrajet.Services.Interfaces
{
    public interface IStationService
    {
        Task<List<StationDto>> GetStationsDto(GetStationsDto query);
        Task<List<Station>> Get();
    }
}
