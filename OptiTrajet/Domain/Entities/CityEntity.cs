using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.Entities
{
    public class CityEntity
    {
        public string Name { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
    }
}
