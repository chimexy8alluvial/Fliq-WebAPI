using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using Fliq.Contracts.Profile;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using Mapster;
using Newtonsoft.Json;

namespace Fliq.Api.Mapping
{
    public class ExploreMappingConfig
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Gender, GenderDto>()
                .Map(dest => dest.GenderType, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            config.NewConfig<Location, LocationDto>()
                .Map(dest => dest.Lat, src => src.Lat)
                .Map(dest => dest.Lng, src => src.Lng);

            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>()
                .Map(dest => dest.PictureUrl, src => src.PictureUrl)
                .Map(dest => dest.Caption, src => src.Caption ?? "");

            config.NewConfig<PromptResponse, ExplorePromptResponseDto>()
                .Map(dest => dest.PromptQuestionId, src => src.PromptQuestionId)
                .Map(dest => dest.Response, src => src.Response);

            config.NewConfig<ProfileType, ProfileTypeDto>()
                .Map(dest => dest.ProfileType, src => (int)src);

            config.NewConfig<BusinessIdentificationDocument, BusinessIdentificationDocumentResponse>()
                .Map(dest => dest.BusinessIdentificationDocumentTypeId, src => src.BusinessIdentificationDocumentTypeId)
                .Map(dest => dest.BusinessIdentificationDocumentType, src => src.BusinessIdentificationDocumentType)
                .Map(dest => dest.BusinessIdentificationDocumentFront, src => src.FrontDocumentUrl)
                .Map(dest => dest.BusinessIdentificationDocumentBack, src => src.BackDocumentUrl);

            config.NewConfig<SexualOrientation, SexualOrientationDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            config.NewConfig<Religion, ReligionDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            config.NewConfig<Occupation, OccupationDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            config.NewConfig<EducationStatus, EducationStatusDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            config.NewConfig<Ethnicity, EthnicityDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsVisible, src => true);

            TypeAdapterConfig<UserProfile, ExploreProfileResponse>.NewConfig()
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.DOB, src => src.DOB)
                .Map(dest => dest.Gender, src => src.Gender.Adapt<GenderDto>())
                .Map(dest => dest.SexualOrientation, src => src.SexualOrientation.Adapt<SexualOrientationDto>())
                .Map(dest => dest.Religion, src => src.Religion.Adapt<ReligionDto>())
                .Map(dest => dest.Occupation, src => src.Occupation.Adapt<OccupationDto>())
                .Map(dest => dest.EducationStatus, src => src.EducationStatus.Adapt<EducationStatusDto>())
                .Map(dest => dest.Ethnicity, src => src.Ethnicity.Adapt<EthnicityDto>())
                .Map(dest => dest.HaveKids, src => src.HaveKids.Adapt<HaveKidsDto>())
                .Map(dest => dest.WantKids, src => src.WantKids.Adapt<WantKidsDto>())
                .Map(dest => dest.Location, src => src.Location.Adapt<LocationDto>())
                .Map(dest => dest.AllowNotifications, src => src.AllowNotifications)
                .Map(dest => dest.Passions, src => MapPassions(src.PassionsJson))
                .Map(dest => dest.ProfileTypes, src => MapProfileTypes(src.ProfileTypeJson))
                .Map(dest => dest.Photos, src => new List<ProfilePhotoResponse>())
                .Map(dest => dest.PromptResponses, src => src.PromptResponses.Adapt<List<ExplorePromptResponseDto>>())
                .Map(dest => dest.BusinessIdentificationDocument, src => src.BusinessIdentificationDocument != null
                            ? new BusinessIdentificationDocumentResponse(
                                src.BusinessIdentificationDocument.Id,
                                src.BusinessIdentificationDocument.BusinessIdentificationDocumentType,
                                src.BusinessIdentificationDocument.FrontDocumentUrl,
                                src.BusinessIdentificationDocument.BackDocumentUrl
                            )            
                            : null);
           
            // Mapping for PaginationResponse<UserProfile> to PaginationResponse<ExploreProfileResponse>
            TypeAdapterConfig<PaginationResponse<UserProfile>, PaginationResponse<ExploreProfileResponse>>.NewConfig()
            .MapWith(src => new PaginationResponse<ExploreProfileResponse>(
                src.Data.Select(up => up.Adapt<ExploreProfileResponse>()).ToList(),
                src.TotalCount,
                src.PageNumber,
                src.PageSize
            ));

            // Mapping for ExploreResult to Fliq.Contracts.Explore.ExploreResponse
            TypeAdapterConfig<ExploreResult, ExploreResponse>.NewConfig()
            .Map(dest => dest.Data, src => src.UserProfiles.Adapt<PaginationResponse<ExploreProfileResponse>>());


            config.NewConfig<ExploreQuery, ExploreRequest>();
            config.NewConfig<ExploreEventsQuery, ExploreEventsRequest>();
            config.NewConfig<ExploreEventsResult, ExploreEventsResponse>();
        } // Helper methods for complex mappings
            private static List<string> MapPassions(string? passionsJson)
            {
                return JsonConvert.DeserializeObject<List<string>>(passionsJson ?? "[]") ?? [];
            }

            private static List<ProfileTypeDto> MapProfileTypes(string? profileTypeJson)
            {
                var profileTypes = JsonConvert.DeserializeObject<List<ProfileType>>(profileTypeJson ?? "[]");
                return profileTypes?.Adapt<List<ProfileTypeDto>>() ?? [];
            }

    }
}