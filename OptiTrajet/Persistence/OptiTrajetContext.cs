using Microsoft.EntityFrameworkCore;
using OptiTrajet.Domain.Entities;

namespace OptiTrajet.Persistence
{
    public class OptiTrajetContext : DbContext
    {
        public OptiTrajetContext(DbContextOptions<OptiTrajetContext> options) : base(options) { }
        public DbSet<Place> Places { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
