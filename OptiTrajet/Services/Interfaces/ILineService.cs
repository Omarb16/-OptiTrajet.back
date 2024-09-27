using OptiTrajet.Domain.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface ILineService
    {
        Task<LineDto[]> Get();
    }
}
