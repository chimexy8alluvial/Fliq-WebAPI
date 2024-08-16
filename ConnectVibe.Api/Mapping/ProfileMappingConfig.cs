using ConnectVibe.Application.Authentication.Common.Profile;
using ConnectVibe.Application.Profile.Commands.Create;
using ConnectVibe.Contracts.Profile;
using ConnectVibe.Domain.Entities.Profile;
using Mapster;

namespace ConnectVibe.Api.Mapping
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
            config.NewConfig<UserProfile, ProfileResponse>();
            config.NewConfig<CreateProfileResult, ProfileResponse>();
            //config.ForType<IFormFile, IFormFile>()
            //    .MapWith(src => new IFormFile(src));
        }

        private List<ProfilePhotoDto> MapPhotos(List<ProfilePhotoDto> photoDtos)
        {
            // Implement logic to convert ProfilePhotoDto to ProfilePhoto
            // For example, save files to disk and create ProfilePhoto objects
            List<ProfilePhotoDto> photos = new();

            foreach (var photoDto in photoDtos)
            {
                photos.Add(new ProfilePhotoDto
                {
                    Caption = photoDto.Caption,
                    ImageFile = photoDto.ImageFile
                });
            }

            return photos;
        }
    }
}