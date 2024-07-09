using OptiTrajet.Domain.Entities;

namespace OptiTrajet.Services.Interfaces
{
    public interface IPlaceService
    {
        Task<Place> Add(AddPlace command);
    }
}
