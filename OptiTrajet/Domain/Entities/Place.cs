using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.Entities
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, Column(TypeName = "decimal(18,16)")]
        public decimal Lat { get; set; }
        [Required, Column(TypeName = "decimal(18,16)")]
        public decimal Lon { get; set; }
        [MaxLength(250)]
        public string Adresse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
