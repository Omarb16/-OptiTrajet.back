using OptiTrajet.Domain.Entities;
using OptiTrajet.Dtos.In;
using OptiTrajet.Dtos.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface IStationService
    {
        Task<List<StationDto>> Get(GetStations command);
    }
}
