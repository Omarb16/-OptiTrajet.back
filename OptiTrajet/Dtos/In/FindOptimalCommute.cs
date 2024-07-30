﻿namespace OptiTrajet.Dtos.In
{
    public class FindOptimalCommute
    {
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public int Duration { get; set; }
        public int Radius { get; set; }
        public Guid[] Lines { get; set; } = Array.Empty<Guid>();
    }
}
