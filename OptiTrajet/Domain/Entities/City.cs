using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.Entities
{
    public class City
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(10)]
        public string CodePostal { get; set; } = string.Empty;
        [Required, MaxLength(5000)]
        public string Coordianates { get; set; } = string.Empty;
    }
}
