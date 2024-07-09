using OptiTrajet.Dtos;

namespace OptiTrajet.Services.Interfaces
{
    public interface ICityService
    {
        Task<List<CityDto>> Get();
    }
}
