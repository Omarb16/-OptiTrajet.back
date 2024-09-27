namespace OptiTrajet.Domain.In
{
    public class GetStations
    {
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public Guid[] Lines { get; set; } = Array.Empty<Guid>();
        public int Radius { get; set; } = 0;
    }
}
