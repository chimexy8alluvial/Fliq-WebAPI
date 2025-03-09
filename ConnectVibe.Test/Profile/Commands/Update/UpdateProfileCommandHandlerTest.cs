using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Profile.Commands.Update;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Fliq.Test.Profile.Commands.Update

{
    [TestClass]
    public class UpdateProfileCommandHandlerTests
    {
        private Mock<IMapper>? _mapperMock;
        private Mock<IMediaServices>? _mediaServiceMock;
        private Mock<IProfileRepository>? _profileRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILocationService>? _locationServiceMock;
        private Mock<IHttpContextAccessor>? _httpContextAccessorMock;
        private Mock<ILoggerManager>? _loggerManagerMock;
        private UpdateProfileCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _mediaServiceMock = new Mock<IMediaServices>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _loggerManagerMock = new Mock<ILoggerManager>();

            _handler = new UpdateProfileCommandHandler(
                _mapperMock.Object,
                _mediaServiceMock.Object,
                _profileRepositoryMock.Object,
                _userRepositoryMock.Object,
                _locationServiceMock.Object,
                _httpContextAccessorMock.Object,
                _loggerManagerMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new UpdateProfileCommand
            {
                UserId = 1,
                Photos = null,
                Location = null
            };

            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>()))
                .Returns(new User { Id = 1 });

            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>()))
                .Returns((UserProfile?)null);

            _loggerManagerMock?.Setup(x => x.LogError(It.IsAny<string>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.FirstError);

            // Verify logger was called
            _loggerManagerMock?.Verify(x => x.LogError("User profile not found"), Times.Once);
        }


        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new UpdateProfileCommand();
            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_NoProfileForUser_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new UpdateProfileCommand();
            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(new User());
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>())).Returns((UserProfile)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PhotosProvided_UploadsPhotosAndUpdatesProfile()
        {
            // Arrange
            var command = new UpdateProfileCommand
            {
                Photos = new List<ProfilePhotoMapped>
                {
                    new ProfilePhotoMapped { ImageFile = null, Caption = "Photo1" },
                    new ProfilePhotoMapped { ImageFile = null, Caption = "Photo2" }
                }
            };
            var userProfile = new UserProfile { Photos = new List<ProfilePhoto>() };

            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(new User());
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>())).Returns(userProfile);

            _mediaServiceMock?.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("https://someurl.com/image.jpg");

            _mapperMock?.Setup(x => x.Map<UserProfile>(It.IsAny<UpdateProfileCommand>()))
                .Returns(userProfile);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _profileRepositoryMock?.Verify(x => x.Update(It.IsAny<UserProfile>()), Times.Once);
            _mediaServiceMock?.Verify(x => x.UploadImageAsync(It.IsAny<IFormFile>()), Times.Exactly(2));
            _loggerManagerMock?.Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task Handle_PhotoUploadFails_ReturnsInvalidImageError()
        {
            // Arrange
            var command = new UpdateProfileCommand
            {
                Photos = new List<ProfilePhotoMapped>
                {
                    new ProfilePhotoMapped { ImageFile = null, Caption = "Photo1" }
                }
            };
            var userProfile = new UserProfile { Photos = new List<ProfilePhoto>() };

            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(new User());
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>())).Returns(userProfile);
            string? response=null;
            _mediaServiceMock?.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Image.InvalidImage, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_LocationProvided_UpdatesLocation()
        {
            // Arrange
            var command = new UpdateProfileCommand
            {
                Location = new Location { Lat = 50.0, Lng = 10.0 }
            };
            var userProfile = new UserProfile { Location = new Location() };

            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(new User());
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>())).Returns(userProfile);

            _locationServiceMock?.Setup(x => x.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(new LocationQueryResponse());

            _mapperMock?.Setup(x => x.Map<LocationDetail>(It.IsAny<LocationQueryResponse>()))
                .Returns(new LocationDetail());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _profileRepositoryMock?.Verify(x => x.Update(It.IsAny<UserProfile>()), Times.Once);
            _locationServiceMock?.Verify(x => x.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UpdatesProfileSuccessfully()
        {
            // Arrange
            var command = new UpdateProfileCommand
            {
                ProfileDescription = "New description"
            };
            var userProfile = new UserProfile { ProfileDescription = "Old description" };

            _httpContextAccessorMock?.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            _userRepositoryMock?.Setup(x => x.GetUserById(It.IsAny<int>())).Returns(new User());
            _profileRepositoryMock?.Setup(x => x.GetProfileByUserId(It.IsAny<int>())).Returns(userProfile);

            _mapperMock?.Setup(x => x.Map<UpdateProfileCommand, UserProfile>(command, userProfile))
                .Returns(userProfile);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _profileRepositoryMock?.Verify(x => x.Update(userProfile), Times.Once);
            _loggerManagerMock?.Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}