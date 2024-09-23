using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Profile
{
    public class EducationStatus
    {
        public int Id { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum EducationLevel
    {
        HighSchool,
        AssociateDegree,
        BachelorDegree,
        MasterDegree,
        Doctorate,
        Other,
        PreferNotToSay
    }
}
