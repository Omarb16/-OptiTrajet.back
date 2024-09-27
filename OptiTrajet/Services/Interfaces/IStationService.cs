using OptiTrajet.Domain.In;
using OptiTrajet.Domain.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface IStationService
    {
        Task<StationDto[]> Get(GetStations command);
    }
}
