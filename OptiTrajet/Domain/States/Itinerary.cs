using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OptiTrajet.Domain.States
{
    public class Itinerary
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(Station))]
        public Guid StationId { get; set; }
        public virtual Station Station { get; set; }
        [ForeignKey(nameof(Place))]
        public Guid PlaceId { get; set; }
        public virtual Place Place { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
