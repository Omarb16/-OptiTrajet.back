using OptiTrajet.Domain.In;
using Spire.Xls;

namespace OptiTrajet.Services.Interfaces
{
    public interface IItineraryService
    {
        Task FindItineraries(FindOptimalCommute command);
        Task<MemoryStream> GetReport(GetReport command);
    }
}
