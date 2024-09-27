namespace OptiTrajet.Domain.Entities
{
    public class StationLatLon
    {
        public Guid Id { get; set; }
        public decimal Lat { get; set; } = 0;
        public decimal Lon { get; set; } = 0;
    }
}
