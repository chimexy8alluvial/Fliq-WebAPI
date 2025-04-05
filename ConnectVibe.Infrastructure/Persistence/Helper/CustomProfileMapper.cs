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
                    GenderType = firstRow.GenderType,
                },
                Occupation = new Occupation
                {
                    Id = firstRow.OccupationId,
                    OccupationName = firstRow.OccupationName,
                },
                EducationStatus = new EducationStatus
                {
                    Id = firstRow.EducationStatusId,
                    EducationLevel = firstRow.EducationLevel,
                },
                SexualOrientation = new SexualOrientation
                {
                    Id = firstRow.SexualOrientationId,
                    SexualOrientationType = firstRow.SexualOrientationType,
                },
                Religion = new Religion
                {
                    Id = firstRow.ReligionId,
                    ReligionType = firstRow.ReligionType,
                },
                Ethnicity = new Ethnicity
                {
                    Id = firstRow.EthnicityId,
                    EthnicityType = firstRow.EthnicityType,
                },
                WantKids = new WantKids
                {
                    Id = firstRow.WantKidsId,
                    WantKidsType = firstRow.WantKidsType,
                    
                },
                HaveKids = new HaveKids
                {
                    Id = firstRow.HaveKidsId,
                    HaveKidsType = firstRow.HaveKidsType,
                   
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
