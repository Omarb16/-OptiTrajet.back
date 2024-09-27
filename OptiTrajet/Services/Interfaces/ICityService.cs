using OptiTrajet.Domain.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface ICityService
    {
        Task<CityDto[]> Get();
    }
}
