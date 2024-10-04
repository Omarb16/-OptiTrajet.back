namespace OptiTrajet.Domain.Entities
{
    public class ReportEntity
    {
        public string City { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
        public string Station { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}
