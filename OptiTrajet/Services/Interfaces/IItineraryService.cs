namespace OptiTrajet.Services.Interfaces
{
    public interface IItineraryService
    {
        Task FindOptimalCommute(FindOptimalCommute command);
    }
}
