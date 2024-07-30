namespace OptiTrajet.Dtos.Out
{
    public class StationLatLonDto
    {
        public Guid Id { get; set; }
        public decimal Lat { get; set; } = 0;
        public decimal Lon { get; set; } = 0;
    }
}
