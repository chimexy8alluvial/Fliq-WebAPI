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
                .Ignore(dest => dest.Photos).Ignore(dest => dest.PromptResponses)
                .Map(dest => dest.ProfileTypes,
            src => src.ProfileTypes.Select(dto => (ProfileType)dto.ProfileType).ToList());

            config.NewConfig<CreateProfileCommand, UserProfile>().Ignore(dest => dest.Photos)
                .Map(dest => dest.ProfileTypes, src => src.ProfileTypes);

            config.NewConfig<EthnicityDto, Ethnicity>()
                .Map(dest => dest.EthnicityType, src => (EthnicityType)src.EthnicityType);
            config.NewConfig<GenderDto, Gender>()
                .Map(dest => dest.GenderType, src => (GenderType)src.GenderType);
            config.NewConfig<HaveKidsDto, HaveKids>()
                .Map(dest => dest.HaveKidsType, src => (HaveKidsType)src.HaveKidsType);
            config.NewConfig<WantKidsDto, WantKids>()
                .Map(dest => dest.WantKidsType, src => (WantKidsType)src.WantKidsType);
            config.NewConfig<ReligionDto, Religion>()
                .Map(dest => dest.ReligionType, src => (ReligionType)src.ReligionType);
            config.NewConfig<SexualOrientationDto, SexualOrientation>()
                .Map(dest => dest.SexualOrientationType, src => (SexualOrientationType)src.SexualOrientationType);
            config.NewConfig<EducationStatusDto, EducationStatus>()
                .Map(dest => dest.EducationLevel, src => (EducationLevel)src.EducationLevel);
            config.NewConfig<Occupation, OccupationDto>()
                .Map(dest => dest.OccupationName, src => src.OccupationName);
            config.NewConfig<CreatePromptResponseDto, PromptResponseDto>(); //Add prompt response mapping
            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>();
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
                .Map(dest => dest.EthnicityType, src => (EthnicityType?)src.EthnicityType);
            config.NewConfig<UpdateGenderDto, Gender>()
                .IgnoreNullValues(true)
                .Map(dest => dest.GenderType, src => (GenderType?)src.GenderType);
            config.NewConfig<UpdateHaveKidsDto, HaveKids>().IgnoreNullValues(true)
                .Map(dest => dest.HaveKidsType, src => (HaveKidsType?)src.HaveKidsType);
            config.NewConfig<UpdateWantKidsDto, WantKids>().IgnoreNullValues(true)
                .Map(dest => dest.WantKidsType, src => (WantKidsType?)src.WantKidsType);
            config.NewConfig<UpdateReligionDto, Religion>().IgnoreNullValues(true)
                .Map(dest => dest.ReligionType, src => (ReligionType?)src.ReligionType);
            config.NewConfig<UpdateSexualOrientationDto, SexualOrientation>().IgnoreNullValues(true)
                .Map(dest => dest.SexualOrientationType, src => (SexualOrientationType?)src.SexualOrientationType);
            config.NewConfig<UpdateEducationStatusDto, EducationStatus>().IgnoreNullValues(true)
                .Map(dest => dest.EducationLevel, src => (EducationLevel?)src.EducationLevel);
            config.NewConfig<UpdateOccupationDto, OccupationDto>().IgnoreNullValues(true)
                .Map(dest => dest.OccupationName, src => src.OccupationName);
            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>();
            config.NewConfig<UpdateProfilePhotoDto, ProfilePhotoMapped>()
                .IgnoreNullValues(true)
                .Map(dest => dest.ImageFile, src => src.ImageFile)
                 .AfterMapping(async (src, dest) =>
                 {
                     dest.ImageFile = await CloneFile(src.ImageFile);
                 });
            //config.NewConfig<UserProfile, ProfileResponse>();
            config.NewConfig<CreateProfileResult, UpdateProfileResponse>()
                .Map(dest => dest, src => src.Profile)
                .Map(dest => dest.DOB, src => src.Profile.DOB);

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