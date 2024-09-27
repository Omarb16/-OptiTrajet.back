namespace OptiTrajet.Populate.Model
{
    public class StationIDFMobilites
    {
        public PositionIDFMobilites geo_point_2d { get; set; }
        public int id_gares { get; set; }
        public string nom_gares { get; set; } = string.Empty;
        public string res_com { get; set; } = string.Empty;
    }

    public class PositionIDFMobilites
    {
        public decimal lat { get; set; }
        public decimal lon { get; set; }
    }

    public class Station
    {
        public Position Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
    }

    public class Position
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    // ----------------------------------------------------------------------------------

    public class CityIDF
    {
        public List<Feature> features { get; set; } = new List<Feature>(0);
    }

    public class Feature
    {
        public Property properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Property
    {
        public string nom { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }

    public class Geometry
    {
        public decimal[][][] coordinates { get; set; } = Array.Empty<decimal[][]>();
    }

    public class City
    {
        public string name { get; set; } = string.Empty;
        public string codepostal { get; set; } = string.Empty;
        public decimal[][] coordinates { get; set; } = Array.Empty<decimal[]>();
    }
}
