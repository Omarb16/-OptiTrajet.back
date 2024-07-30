using OptiTrajet.Dtos.Out;

namespace OptiTrajet.Services.Interfaces
{
    public interface ILineService
    {
        Task<List<LineDto>> Get();
    }
}
