using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Models;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Commands.Update;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Profile;
using Fliq.Contracts.Profile.UpdateDtos;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class ProfileMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            
        config.NewConfig<CreateProfileRequest, CreateProfileCommand>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Photos).Ignore(dest => dest.PromptResponses)
            .Map(dest => dest.ProfileTypes,
                src => src.ProfileTypes != null
                    ? src.ProfileTypes.Select(dto => (ProfileType)dto.ProfileType).ToList()
                    : new List<ProfileType>())
            .Map(dest => dest.CurrentSection, src => (ProfileSection)src.CurrentSection);
      

            config.NewConfig<CreateProfileCommand, UserProfile>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Photos).Ignore(dest => dest.PromptResponses)
                .Map(dest => dest.ProfileTypes, src => src.ProfileTypes);

            config.NewConfig<CreatePromptResponseDto, PromptResponseDto>(); //Add prompt response mapping
            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>();
            config.NewConfig<BusinessIdentificationDocument, BusinessIdentificationDocumentResponse>();
            config.NewConfig<BusinessIdentificationDocumentDto, BusinessIdentificationDocumentMapped>()
                .Map(dest => dest.BusinessIdentificationDocumentFront, src => src.BusinessIdentificationDocumentFront)
                .Map(dest => dest.BusinessIdentificationDocumentBack, src => src.BusinessIdentificationDocumentBack);
            config.NewConfig<ProfilePhotoDto, ProfilePhotoMapped>()
                .Map(dest => dest.ImageFile, src => src.ImageFile)
                 .AfterMapping(async (src, dest) =>
                 {
                     dest.ImageFile = await CloneFile(src.ImageFile);
                 });

            config.NewConfig<UserProfile, ProfileResponse>()
                .Map(dest => dest.ProfileTypes, src => src.ProfileTypes
                    .Select(pt => new ProfileTypeDto((int)pt)).ToList());

            config.NewConfig<ProfileTypeDto, ProfileType>()
                .Map(dest => dest, src => (ProfileType)src.ProfileType);

            config.NewConfig<CreateProfileResult, ProfileResponse>()
                .Map(dest => dest, src => src.Profile)
                .Map(dest => dest.DOB, src => src.Profile.DOB);

            config.NewConfig<LocationQueryResponse, LocationDetail>();

            //Update Profile
            config.NewConfig<UpdateProfileRequest, UpdateProfileCommand>()
                .IgnoreNullValues(true) // This will ignore null values in general
                .Ignore(dest => dest.Photos) // Assuming photos are being handled elsewhere
                .Map(dest => dest.ProfileTypes,
                 src => src.profileTypes != null
                 ? src.profileTypes.Select(dto => (ProfileType)dto.ProfileType).ToList()
                : new List<ProfileType>()); // Handle null collection by providing an empty list

            config.NewConfig<UpdateProfileCommand, UserProfile>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Photos)
                .Map(dest => dest.ProfileTypes, src => src.ProfileTypes);

            config.NewConfig<UpdateEthnicityDto, Ethnicity>().IgnoreNullValues(true)
                .Map(dest => dest.EthnicityType, src => src.EthnicityType);
          
            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>();
            config.NewConfig<UpdateProfilePhotoDto, ProfilePhotoMapped>()
                .IgnoreNullValues(true)
                .Map(dest => dest.ImageFile, src => src.ImageFile)
                 .AfterMapping(async (src, dest) =>
                 {
                     dest.ImageFile = await CloneFile(src.ImageFile);
                 });
            config.NewConfig<BusinessIdentificationDocument, BusinessIdentificationDocumentResponse>();
            config.NewConfig<UpdateBusinessIdentificationDocumentDto, BusinessIdentificationDocumentMapped>()
                .IgnoreNullValues(true)
                .Map(dest => dest.BusinessIdentificationDocumentFront, src => src.BusinessIdentificationDocumentFront)
                .Map(dest => dest.BusinessIdentificationDocumentBack, src => src.BusinessIdentificationDocumentBack);
            //config.NewConfig<UserProfile, ProfileResponse>();
            config.NewConfig<CreateProfileResult, ReadProfileResponse>()
                .Map(dest => dest, src => src.Profile)
                .Map(dest => dest.DOB, src => src.Profile.DOB)
                .Map(dest => dest.Gender, src => new ReadGenderDto(
                     src.Profile.GenderId,
                     src.Profile.Gender.GenderType, null))
                .Map(dest => dest.SexualOrientation, src => new ReadSexualOrientationDto(
                    src.Profile.SexualOrientationId ?? 0,
                    src.Profile.SexualOrientation.SexualOrientationType,
                    src.Profile.IsSexualOrientationVisible))
               .Map(dest => dest.Ethnicity, src => new ReadEthnicityDto(
                    src.Profile.EthnicityId ?? 0,
                    src.Profile.Ethnicity.EthnicityType,
                    src.Profile.IsEthnicityVisible))
                .Map(dest => dest.EducationStatus, src => new ReadEducationStatusDto(
                    src.Profile.EducationStatusId ?? 0,
                    src.Profile.EducationStatus.EducationLevel,
                    src.Profile.IsEducationStatusVisible))
                .Map(dest => dest.Religion, src => new ReadReligionDto(
                    src.Profile.ReligionId ?? 0,
                    src.Profile.Religion.ReligionType,
                    src.Profile.IsReligionVisible))
            .Map(dest => dest.WantKids, src => new ReadWantKidsDto(
                    src.Profile.WantKidsId,
                    src.Profile.WantKids.WantKidsType,
                    null))
            .Map(dest => dest.HaveKids, src => new ReadHaveKidsDto(
                    src.Profile.WantKidsId,
                    src.Profile.HaveKids.HaveKidsType,
                    null))
            .Map(dest => dest.Photos, src => src.Profile.Photos)
            .Map(dest => dest.Passions, src => src.Profile.Passions)
            .Map(dest => dest.PromptResponses, src => src.Profile.PromptResponses);

            config.NewConfig<PaginationResponse<UserProfile>, PaginationResponse<ProfileResponse>>().IgnoreNullValues(true)
                 .Map(dest => dest.Data, src => src.Data.Select(userProfile => userProfile.Adapt<ProfileResponse>()).ToList());

            config.NewConfig<UserProfile, ExploreProfileResponse>()
    .Map(dest => dest.PromptResponses,
         src => src.PromptResponses.Select(pr => new ExplorePromptResponseDto(
             pr.PromptQuestionId,
             pr.Response
         )).ToList());

        }

        public static async Task<IFormFile> CloneFile(IFormFile file)
        {
            if (file == null) return null;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            // Set the position to 0 to allow reading from the start
            memoryStream.Position = 0;

            return new FormFile(memoryStream, 0, memoryStream.Length, file.Name, Path.GetFileName(file.FileName))
            {
                ContentType = file.ContentType,
                Headers = file.Headers
            };
        }
    }
}