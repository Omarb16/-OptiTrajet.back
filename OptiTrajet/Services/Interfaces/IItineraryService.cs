using OptiTrajet.Domain.In;
using Spire.Xls;

namespace OptiTrajet.Services.Interfaces
{
    public interface IItineraryService
    {
        Task FindOptimalCommute(FindOptimalCommute command);
        Task<MemoryStream> GetReport(GetReport command);
    }
}
