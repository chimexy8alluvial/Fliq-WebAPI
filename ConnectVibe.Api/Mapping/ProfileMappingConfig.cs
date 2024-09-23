﻿using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Models;
using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Profile;
using Fliq.Domain.Entities.Profile;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class ProfileMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateProfileRequest, CreateProfileCommand>()
                .Ignore(dest => dest.Photos);

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
            config.NewConfig<ProfilePhoto, ProfilePhotoResponse>();
            config.NewConfig<ProfilePhotoDto, ProfilePhotoMapped>()
                .Map(dest => dest.ImageFile, src => src.ImageFile)
                 .AfterMapping(async (src, dest) =>
                 {
                     dest.ImageFile = await CloneFile(src.ImageFile); // Note: Avoid blocking calls in real scenarios
                 });
            config.NewConfig<UserProfile, ProfileResponse>();
            config.NewConfig<CreateProfileResult, ProfileResponse>()
                .Map(dest => dest, src => src.Profile)
                .Map(dest => dest.DOB, src => src.Profile.DOB);
            config.NewConfig<LocationQueryResponse, LocationDetail>();
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