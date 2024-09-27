namespace OptiTrajet.Domain.Out
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal[][] Coordianates { get; set; } = new decimal[2][];
    }
}
