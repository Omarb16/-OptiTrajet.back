namespace OptiTrajet.Domain.Out
{
    public class StationDto
    {
        public Guid Id { get; set; }
        public int Duration { get; set; }
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
