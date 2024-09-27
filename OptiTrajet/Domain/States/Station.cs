using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.States
{
    public class Station
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required,MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, Column(TypeName = "decimal(18,3)")]    
        public decimal Lat { get; set; }
        [Required, Column(TypeName = "decimal(18,3)")]
        public decimal Lon { get; set; }
        [ForeignKey(nameof(Line))]
        public Guid LineId { get; set; }
        public virtual Line Line { get; set; }
        public Guid? CityId { get; set; }
        public virtual City City { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
