namespace OptiTrajet.Domain.In
{
    public class GetReport
    {
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public int Duration { get; set; }
    }
}
