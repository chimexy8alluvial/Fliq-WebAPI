﻿namespace ConnectVibe.Domain.Entities.Profile
{
    public class Religion
    {
        public int Id { get; set; }
        public ReligionType ReligionType { get; set; }
        public bool IsVisible { get; set; } = false;
    }

    public enum ReligionType
    {
        Christianity,
        Islam,
        Others
    }
}