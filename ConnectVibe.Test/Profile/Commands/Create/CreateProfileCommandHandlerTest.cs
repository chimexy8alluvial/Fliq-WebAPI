using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
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
        private Mock<IProfileRepository> _profileRepositoryMock;
        private Mock<ISettingsRepository> _settingsRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ClaimsPrincipal> _claimsPrincipalMock;
        private Mock<IPromptQuestionRepository> _promptQuestionRepositoryMock;
        private Mock<IPromptCategoryRepository> _promptCategoryRepositoryMock;
        private Mock<IPromptResponseRepository> _promptResponseRepositoryMock;
        private Mock<ILoggerManager> _loggerManagerMock;
        private Mock<IMediaServices> _mediaServicesMock;

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _settingsRepositoryMock = new Mock<ISettingsRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _settingsRepositoryMock = new Mock<ISettingsRepository>();
            _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            _promptQuestionRepositoryMock = new Mock<IPromptQuestionRepository>();
            _promptCategoryRepositoryMock = new Mock<IPromptCategoryRepository>();
            _loggerManagerMock = new Mock<ILoggerManager>();
            _mediaServicesMock = new Mock<IMediaServices>();
            _promptResponseRepositoryMock = new Mock<IPromptResponseRepository>();

            // Mocking HttpContext to return a valid user ID
            _httpContextAccessorMock.Setup(x => x.HttpContext.User)
                .Returns(_claimsPrincipalMock.Object);

            _handler = new CreateProfileCommandHandler(
                _mapperMock.Object,
                _profileRepositoryMock.Object,
                _userRepositoryMock.Object,
                _locationServiceMock.Object,
                _settingsRepositoryMock.Object,
                _loggerManagerMock.Object,
                _promptQuestionRepositoryMock.Object,
                _promptCategoryRepositoryMock.Object,
                _mediaServicesMock.Object,
                _promptResponseRepositoryMock.Object);
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
        public async Task Handle_ValidCommand_CreatesUserProfileSuccessfully()
        {
            // Arrange
            var location = new Location { Lat = 51.5074, Lng = -0.1278, IsVisible = true };
            var profileDescription = "I am a software engineer who admires hardworking women in tech description";
            var command = new CreateProfileCommand
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = profileDescription,
                Photos =
                [
                    new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }
                ],
                Location = location,
            };

            var userProfile = new UserProfile
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "I am a software engineer who admires hardworking women in tech description",
                UserId = 1,
            };

            var user = new User { Id = 1 };

            // Mocking the Location Service Response
            var locationResponse = new LocationQueryResponse
            {
                PlusCode = new Fliq.Application.Common.Models.PlusCode { CompoundCode = "FakeCode", GlobalCode = "GlobalCode123" },
                Results = new List<Result>
                {
                    new Result
                    {
                        FormattedAddress = "123 Fake Street, Faketown, Fakestate",
                        Geometry = new Fliq.Application.Common.Models.Geometry
                        {
                            Location = new Fliq.Application.Common.Models.Locationn
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

            _profileRepositoryMock.Setup(repo => repo.GetUserProfileByUserId(It.IsAny<int>()))
                .Returns((UserProfile?)null);

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);

            _mediaServicesMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
              .ReturnsAsync("image.jpeg");

            _mapperMock.Setup(mapper => mapper.Map<LocationDetail>(new LocationQueryResponse()));

            _mapperMock.Setup(mapper => mapper.Map<UserProfile>(command))
                .Returns(userProfile);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(profileDescription, result.Value.Profile.ProfileDescription);
            _profileRepositoryMock.Verify(repo => repo.Add(It.IsAny<UserProfile>()), Times.Once);
        }

        private IFormFile CreateMockFormFile()
        {
            var fileMock = new Mock<IFormFile>();

            // Mock file content
            var content = "Fake file content";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Set up the stream for reading
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);

            // Setup file properties
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");
            fileMock.Setup(_ => _.Name).Returns("file");
            fileMock.Setup(_ => _.Headers).Returns(new HeaderDictionary()); // Headers mock

            // Mock the CopyTo method (synchronous)
            fileMock.Setup(_ => _.CopyTo(It.IsAny<Stream>()))
                .Callback<Stream>(stream => ms.CopyTo(stream));

            // Mock the CopyToAsync method (asynchronous)
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns<Stream, CancellationToken>((stream, token) => ms.CopyToAsync(stream, token));

            return fileMock.Object;
        }
    }
}