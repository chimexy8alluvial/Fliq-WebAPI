using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using Fliq.Infrastructure.Persistence.Repositories;
using MapsterMapper;
using Moq;
using Fliq.Domain.Entities;
using Fliq.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Fliq.Application.DatingEnvironment.Common;
using System.Text;
using Fliq.Domain.Common.Errors;

namespace Fliq.Test.Dating.Commands.SpeedDating
{
    [TestClass]
    public class CreateSpeedDatingEventCommandHandlerTests
    {
        private Mock<ISpeedDatingEventRepository>? _mockSpeedDateRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private Mock<IMapper>? _mockMapper;
        private Mock<ILocationService>? _mockLocationService;
        private Mock<IMediaServices>? _mockMediaServices;
        private Mock<ISpeedDateParticipantRepository>? _mockSpeedDateParticipantRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private CreateSpeedDatingEventCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSpeedDateRepository = new Mock<ISpeedDatingEventRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();
            _mockMapper = new Mock<IMapper>();
            _mockLocationService = new Mock<ILocationService>();
            _mockMediaServices = new Mock<IMediaServices>();
            _mockSpeedDateParticipantRepository = new Mock<ISpeedDateParticipantRepository>();
            _mockUserRepository = new Mock<IUserRepository>();

            _handler = new CreateSpeedDatingEventCommandHandler(
                _mockSpeedDateRepository.Object,
                _mockLoggerManager.Object,
                _mockMapper.Object,
                _mockLocationService.Object,
                _mockMediaServices.Object,
                _mockSpeedDateParticipantRepository.Object,
                _mockUserRepository.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidRequest_CreatesSpeedDatingEventSuccessfully()
        {
            // Arrange
            var user = new User { Id = 1001, RoleId = 3 };
            var location = new Location { Lat = 40.7128, Lng = -74.0060, IsVisible = true };
            var locationDetail = new LocationDetail();
            var command = new CreateSpeedDatingEventCommand(
                CreatedByUserId: 1001,
                Title: "Valentine's Speed Dating",
                Category:  SpeedDatingCategory.Heterosexual,
                StartTime: DateTime.UtcNow.AddDays(2),
                MinAge: 25,
                MaxAge: 40,
                MaxParticipants: 20,
                DurationPerPairingMinutes: 5,
                SpeedDateImage: new DatePhotoMapped(CreateMockFormFile()),
                Location: location,
                LocationDetail: locationDetail
            );

            var speedDateEntity = new SpeedDatingEvent { Id = 1, Title = "Valentine's Speed Dating" };
            var locationResponse = new LocationQueryResponse { Status = "OK" };

            _mockUserRepository?.Setup(repo => repo.GetUserById(command.CreatedByUserId)).Returns(user);
            _mockMapper?.Setup(m => m.Map<SpeedDatingEvent>(command)).Returns(speedDateEntity);
            _mockLocationService?.Setup(s => s.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng)).ReturnsAsync(locationResponse);
            _mockMediaServices?.Setup(m => m.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("image.jpg");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual(command.Title, result.Value.Title);

            _mockSpeedDateRepository?.Verify(repo => repo.AddAsync(It.IsAny<SpeedDatingEvent>()), Times.Once);
            _mockSpeedDateParticipantRepository?.Verify(repo => repo.AddAsync(It.IsAny<SpeedDatingParticipant>()), Times.Once);
            _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Successfully created Speed Date Event"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new CreateSpeedDatingEventCommand(
                CreatedByUserId: 9999,
                Title: "Valentine's Speed Dating",
                Category: SpeedDatingCategory.Heterosexual,
                StartTime: DateTime.UtcNow.AddDays(2),
                MinAge: 25,
                MaxAge: 40,
                MaxParticipants: 20,
                DurationPerPairingMinutes: 5,
                SpeedDateImage: null,
                Location: new Location { Lat = 40.7128, Lng = -74.0060, IsVisible = true },
                LocationDetail: new LocationDetail()
            );

            _mockUserRepository?.Setup(repo => repo.GetUserById(command.CreatedByUserId)).Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("User with ID"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_LocationServiceFails_StillCreatesSpeedDatingEvent()
        {
            // Arrange
            var user = new User { Id = 1001, RoleId = 3 };
            var command = new CreateSpeedDatingEventCommand(
                CreatedByUserId: 1001,
                Title: "Valentine's Speed Dating",
                Category: SpeedDatingCategory.Heterosexual,
                StartTime: DateTime.UtcNow.AddDays(2),
                MinAge: 25,
                MaxAge: 40,
                MaxParticipants: 20,
                DurationPerPairingMinutes: 5,
                SpeedDateImage: null,
                Location: new Location { Lat = 40.7128, Lng = -74.0060, IsVisible = true },
                LocationDetail: new LocationDetail()
            );

            var speedDateEntity = new SpeedDatingEvent { Id = 1, Title = command.Title };

            _mockUserRepository?.Setup(repo => repo.GetUserById(command.CreatedByUserId)).Returns(user);
            _mockMapper?.Setup(m => m.Map<SpeedDatingEvent>(command)).Returns(speedDateEntity);
            _mockLocationService?.Setup(s => s.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng)).ReturnsAsync((LocationQueryResponse)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            _mockLoggerManager?.Verify(logger => logger.LogWarn(It.Is<string>(msg => msg.Contains("Failed to retrieve location details"))), Times.Once);
        }

        [TestMethod]
        public async Task Handle_NonCreatorUser_DoesNotAddParticipant()
        {
            // Arrange
            var user = new User { Id = 1002, RoleId = 2 }; // Not a creator role
            var command = new CreateSpeedDatingEventCommand(
                CreatedByUserId: 1002,
                Title: "Valentine's Speed Dating",
                Category: SpeedDatingCategory.Heterosexual,
                StartTime: DateTime.UtcNow.AddDays(2),
                MinAge: 25,
                MaxAge: 40,
                MaxParticipants: 20,
                DurationPerPairingMinutes: 5,
                SpeedDateImage: null,
                Location: new Location { Lat = 40.7128, Lng = -74.0060, IsVisible = true },
                LocationDetail: new LocationDetail()
            );

            var speedDateEntity = new SpeedDatingEvent { Id = 1, Title = command.Title };

            _mockUserRepository?.Setup(repo => repo.GetUserById(command.CreatedByUserId)).Returns(user);
            _mockMapper?.Setup(m => m.Map<SpeedDatingEvent>(command)).Returns(speedDateEntity);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, result.Value.Id);
            _mockSpeedDateParticipantRepository?.Verify(repo => repo.AddAsync(It.IsAny<SpeedDatingParticipant>()), Times.Never);
        }


        private static IFormFile CreateMockFormFile()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Fake File Content"));
            return new FormFile(stream, 0, stream.Length, "file", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }

}
