﻿using Fliq.Contracts.Explore;

namespace Fliq.Contracts.Profile
{
    public class LocationDto
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsVisible { get; set; }
        public LocationDetailDto? LocationDetail { get; init; }
    }
}
