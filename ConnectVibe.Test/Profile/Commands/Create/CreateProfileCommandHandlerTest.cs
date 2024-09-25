using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using ConnectVibe.Application.Common.Interfaces.Services.LocationServices;
using ConnectVibe.Application.Common.Models;
using ConnectVibe.Application.Profile.Commands.Create;
using ConnectVibe.Application.Profile.Common;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;
using ConnectVibe.Domain.Entities.Profile;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Text;

namespace Fliq.Test.Profile.Commands.Create
{
    [TestClass]
    public class CreateProfileCommandHandlerTest
    {
        private CreateProfileCommandHandler _handler;
        private Mock<IMapper> _mapperMock;
        private Mock<IImageService> _imageServiceMock;
        private Mock<IProfileRepository> _profileRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ClaimsPrincipal> _claimsPrincipalMock;


        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _imageServiceMock = new Mock<IImageService>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _claimsPrincipalMock = new Mock<ClaimsPrincipal>();

            // Mocking HttpContext to return a valid user ID
            _httpContextAccessorMock.Setup(x => x.HttpContext.User)
                .Returns(_claimsPrincipalMock.Object);

            _handler = new CreateProfileCommandHandler(
                _mapperMock.Object,
                _imageServiceMock.Object,
                _profileRepositoryMock.Object,
                _userRepositoryMock.Object,
                _locationServiceMock.Object,
                _httpContextAccessorMock.Object);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "I am a software engineer who admires hardworking women in tech description",
                Photos =
                [
                    new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }
                ],
            };

            User? user = null;

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.ProfileNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_DuplicateProfile_ReturnsDuplicateProfileError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "I am a software engineer who admires hardworking women in tech description",
                Photos =
                [
                    new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }
                ],
            };

            var user = new User { Id = 1 };
            var existingProfile = new UserProfile { UserId = user.Id };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _profileRepositoryMock.Setup(repo => repo.GetUserProfileByUserId(user.Id))
                .Returns(existingProfile);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.DuplicateProfile, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_CreatesUserProfileSuccessfully()
        {
            // Arrange

            var location = new Location { Lat = 51.5074, Lng = -0.1278, IsVisible = true };

            var command = new CreateProfileCommand
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "I am a software engineer who admires hardworking women in tech description",
                Photos =
                [
                    new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }
                ],
                Location = location,
            };

            var user = new User { Id = 1 };

            // Mocking the Location Service Response
            var locationResponse = new LocationQueryResponse
            {
                PlusCode = new ConnectVibe.Application.Common.Models.PlusCode { CompoundCode = "FakeCode", GlobalCode = "GlobalCode123" },
                Results = new List<Result>
                {
                    new Result
                    {
                        FormattedAddress = "123 Fake Street, Faketown, Fakestate",
                        Geometry = new ConnectVibe.Application.Common.Models.Geometry
                        {
                            Location = new ConnectVibe.Application.Common.Models.Locationn
                            {
                                Lat = 40.7128,
                                Lng = -74.0060
                            }
                        }
                    }
                },
                Status = "OK"
            };


            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _profileRepositoryMock.Setup(repo => repo.GetUserProfileByUserId(user.Id))
                .Returns((UserProfile?)null);

            _imageServiceMock.Setup(service => service.UploadMediaAsync(CreateMockFormFile()))
                .ReturnsAsync("image/jpeg");

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Profile.UserId);
            _profileRepositoryMock.Verify(repo => repo.Add(It.IsAny<UserProfile>()), Times.Once);
        }

        private IFormFile CreateMockFormFile()
        {
            var fileMock = new Mock<IFormFile>();

            // Setup the file's properties
            var content = "Fake file content";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            return fileMock.Object;
        }
    }

 
}
