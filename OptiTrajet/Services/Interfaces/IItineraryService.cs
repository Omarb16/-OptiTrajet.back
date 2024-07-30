using OptiTrajet.Dtos.In;

namespace OptiTrajet.Services.Interfaces
{
    public interface IItineraryService
    {
        Task FindOptimalCommute(FindOptimalCommute command);
        Task<ByteArrayContent> GetReport(Guid placeId);
    }
}
