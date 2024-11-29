using Fliq.Application.Common.Interfaces.Helper;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Newtonsoft.Json;


namespace Fliq.Infrastructure.Persistence.Helper
{
    public class CustomProfileMapper : ICustomProfileMapper
    {
        public UserProfile MapToUserProfile(IGrouping<int, dynamic> groupedRows)
        {
            var firstRow = groupedRows.FirstOrDefault();
            if (firstRow == null) return null;

            return new UserProfile
            {
                Id = firstRow.Id,
                UserId = firstRow.UserId,
                DOB = firstRow.DOB,
                AllowNotifications = firstRow.AllowNotifications,
                PassionsJson = firstRow.Passions,
                ProfileTypeJson = firstRow.ProfileTypes,
                DateCreated = firstRow.DateCreated,
                DateModified = firstRow.DateModified,
                IsDeleted = firstRow.IsDeleted,

                Gender = new Gender
                {
                    Id = firstRow.GenderId,
                    GenderType = (GenderType)firstRow.GenderType,
                    IsVisible = firstRow.GenderVisible
                },
                Occupation = new Occupation
                {
                    Id = firstRow.OccupationId,
                    OccupationName = firstRow.OccupationName,
                    IsVisible = firstRow.OccupationVisible
                },
                EducationStatus = new EducationStatus
                {
                    Id = firstRow.EducationStatusId,
                    EducationLevel = (EducationLevel)firstRow.EducationLevel,
                    IsVisible = firstRow.EducationVisible
                },
                SexualOrientation = new SexualOrientation
                {
                    Id = firstRow.SexualOrientationId,
                    SexualOrientationType = (SexualOrientationType)firstRow.SexualOrientationType,
                    IsVisible = firstRow.SexualOrientationVisible
                },
                Religion = new Religion
                {
                    Id = firstRow.ReligionId,
                    ReligionType = (ReligionType)firstRow.ReligionType,
                    IsVisible = firstRow.ReligionVisible
                },
                Ethnicity = new Ethnicity
                {
                    Id = firstRow.EthnicityId,
                    EthnicityType = (EthnicityType)firstRow.EthnicityType,
                    IsVisible = firstRow.EthnicityVisible
                },
                WantKids = new WantKids
                {
                    Id = firstRow.WantKidsId,
                    WantKidsType = (WantKidsType)firstRow.WantKidsType,
                    IsVisible = firstRow.WantKidsVisible
                },
                HaveKids = new HaveKids
                {
                    Id = firstRow.HaveKidsId,
                    HaveKidsType = (HaveKidsType)firstRow.HaveKidsType,
                    IsVisible = firstRow.HaveKidsVisible
                },
                Location = new Location
                {
                    Id = firstRow.LocationId,
                    Lat = firstRow.Lat,
                    Lng = firstRow.Lng,
                    IsVisible = firstRow.LocationVisible
                },

                // Deserialize PromptResponses from JSON
                PromptResponses = !string.IsNullOrEmpty(firstRow.PromptResponses)
                ? JsonConvert.DeserializeObject<List<PromptResponse>>(firstRow.PromptResponses)
                : new List<PromptResponse>()
            };
        }
    }
}
