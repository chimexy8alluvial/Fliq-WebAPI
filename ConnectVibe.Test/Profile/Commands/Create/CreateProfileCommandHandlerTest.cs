using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
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
            var command = new CreateProfileCommand { UserId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User?)null);

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
            var command = new CreateProfileCommand
            {
                UserId = 1,
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "Test Description",
                Photos = [new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }],
                Location = new Location { Lat = 51.5074, Lng = -0.1278 }
            };

            var user = new User { Id = 1 };
            var locationResponse = new LocationQueryResponse { Status = "OK" };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);

            _mediaServicesMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("image.jpeg");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            _profileRepositoryMock.Verify(repo => repo.Add(It.IsAny<UserProfile>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_PhotosUploadFails_ReturnsInvalidImageError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                Photos = [new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }]
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mediaServicesMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync((string?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Image.InvalidImage, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_LocationServiceFails_ReturnsLocationServiceError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                Location = new Location { Lat = 51.5074, Lng = -0.1278 }
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync((LocationQueryResponse?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.InvalidPayload, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PromptResponsesWithInvalidCategory_ReturnsCategoryNotFoundError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                PromptResponses = [new PromptResponseDto(1, null, "New Answer", CreateMockFormFile(), CreateMockFormFile(), 999, false)]
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _promptCategoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<int>()))
                .Returns((PromptCategory?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Prompts.CategoryNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PromptResponsesWithInvalidQuestion_ReturnsQuestionNotFoundError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                PromptResponses = [new PromptResponseDto(99, "Valid Question", "Valid Answer", CreateMockFormFile(), CreateMockFormFile(), 1, false)]
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _promptCategoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<int>()))
               .Returns(new PromptCategory() { CategoryName = "Category Name", Id = 1 });

            _promptQuestionRepositoryMock.Setup(repo => repo.GetQuestionByIdAsync(It.IsAny<int>()))
                .Returns((PromptQuestion?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Prompts.QuestionNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_PromptResponsesWithNoAnswer_ReturnsAnswerNotProvidedError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                PromptResponses = [new PromptResponseDto(1, null, null, null, null, 999, false)]
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Prompts.AnswerNotProvided, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ExistingProfile_UpdatesProfileSuccessfully()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "Test Description",
                Photos = [new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }],
                Location = new Location { Lat = 51.5074, Lng = -0.1278 }
            };

            var user = new User { Id = 1 };
            var existingProfile = new UserProfile { UserId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(It.IsAny<int>()))
                .Returns(existingProfile);

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(new LocationQueryResponse { Status = "OK" });

            _mediaServicesMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("image.jpeg");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            _profileRepositoryMock.Verify(repo => repo.Update(It.IsAny<UserProfile>()), Times.Once);
        }

        private IFormFile CreateMockFormFile()
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");
            fileMock.Setup(_ => _.Name).Returns("file");
            fileMock.Setup(_ => _.Headers).Returns(new HeaderDictionary());

            fileMock.Setup(_ => _.CopyTo(It.IsAny<Stream>()))
                .Callback<Stream>(stream => ms.CopyTo(stream));

            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns<Stream, CancellationToken>((stream, token) => ms.CopyToAsync(stream, token));

            return fileMock.Object;
        }
    }
}