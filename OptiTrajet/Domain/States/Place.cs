using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.States
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, Column(TypeName = "decimal(18,3)")]
        public decimal Lat { get; set; }
        [Required, Column(TypeName = "decimal(18,3)")]
        public decimal Lon { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
