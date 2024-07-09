using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.Entities
{
    public class Line
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(7)]
        public string Color { get; set; } = string.Empty;
    }
}
