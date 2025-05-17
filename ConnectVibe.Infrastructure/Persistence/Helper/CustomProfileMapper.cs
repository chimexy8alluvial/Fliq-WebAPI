using Fliq.Application.Common.Interfaces.Helper;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Newtonsoft.Json;


namespace Fliq.Infrastructure.Persistence.Helper
{
    public class CustomProfileMapper : ICustomProfileMapper
    {
        public UserProfile MapToUserProfile(dynamic row)
        {
            if (row == null) return null;

            return new UserProfile
            {
                Id = row.Id,
                UserId = row.UserId,
                DOB = row.DOB,
                AllowNotifications = row.AllowNotifications,
                PassionsJson = row.Passions,
                ProfileTypeJson = row.ProfileTypes,
                DateCreated = row.DateCreated,
                DateModified = row.DateModified,
                IsDeleted = row.IsDeleted,
                Gender = new Gender
                {
                    Id = row.GenderId,
                    GenderType = row.GenderType
                },
                Occupation = new Occupation
                {
                    Id = row.OccupationId,
                    OccupationName = row.OccupationName
                },
                EducationStatus = new EducationStatus
                {
                    Id = row.EducationStatusId,
                    EducationLevel = row.EducationLevel
                },
                SexualOrientation = new SexualOrientation
                {
                    Id = row.SexualOrientationId,
                    SexualOrientationType = row.SexualOrientationType
                },
                Religion = new Religion
                {
                    Id = row.ReligionId,
                    ReligionType = row.ReligionType
                },
                Ethnicity = new Ethnicity
                {
                    Id = row.EthnicityId,
                    EthnicityType = row.EthnicityType
                },
                WantKids = new WantKids
                {
                    Id = row.WantKidsId,
                    WantKidsType = row.WantKidsType
                },
                HaveKids = new HaveKids
                {
                    Id = row.HaveKidsId,
                    HaveKidsType = row.HaveKidsType
                },
                Location = new Location
                {
                    Id = row.LocationId,
                    Lat = row.Lat,
                    Lng = row.Lng,
                    IsVisible = row.LocationVisible // Ensure this column exists or remove
                },
                PromptResponses = !string.IsNullOrEmpty(row.PromptResponses)
                    ? JsonConvert.DeserializeObject<List<PromptResponse>>(row.PromptResponses)
                    : new List<PromptResponse>()
            };
        }
    }
}
