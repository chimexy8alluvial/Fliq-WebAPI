using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Commands.Update;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Settings;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fliq.Test.Settings.Commands.Update
{
    [TestClass]
    public class UpdateSettingsCommandHandlerTests
    {
        private Mock<ISettingsRepository> _mockSettingsRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILoggerManager> _mockLogger;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private UpdateSettingsCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSettingsRepository = new Mock<ISettingsRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _handler = new UpdateSettingsCommandHandler(
                _mockSettingsRepository.Object,
                _mockUserRepository.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new UpdateSettingsCommand(
                1,
                ScreenMode.Dark,
                true,
                true,
                "English",
                new List<NotificationPreference>(),
                new Filter(),
                999
            );

            _mockUserRepository
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.User.UserNotFound));
        }

        [TestMethod]
        public async Task Handle_SettingsNotFound_ReturnsSettingsNotFoundError()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

            var command = new UpdateSettingsCommand(
                1,
                ScreenMode.Dark,
                true,
                true,
                "English",
                new List<NotificationPreference>(),
                new Filter(),
                user.Id
            );

            _mockUserRepository
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockSettingsRepository
                .Setup(repo => repo.GetSettingById(It.IsAny<int>()))
                .Returns((Setting)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Settings.SettingsNotFound));
        }

        [TestMethod]
        public async Task Handle_ValidCommand_UpdatesSettingsAndReturnsResult()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var settings = new Setting
            {
                Id = 1,
                ScreenMode = ScreenMode.White,
                RelationAvailability = false,
                ShowMusicAndGameStatus = false,
                Language = "English",
                NotificationPreferences = new List<NotificationPreference>(),
                Filter = new Filter()
            };

            var command = new UpdateSettingsCommand(
                1,
                ScreenMode.Dark,
                true,
                true,
                "French",
                new List<NotificationPreference>(),
                new Filter(),
                user.Id
            );

            _mockUserRepository
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockSettingsRepository
                .Setup(repo => repo.GetSettingById(It.IsAny<int>()))
                .Returns(settings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            var updatedSettings = result.Value;

            Assert.AreEqual(settings.Id, updatedSettings.Id);
            Assert.AreEqual(ScreenMode.Dark, updatedSettings.ScreenMode);
            Assert.IsTrue(updatedSettings.RelationAvailability);
            Assert.IsTrue(updatedSettings.ShowMusicAndGameStatus);
            Assert.AreEqual("French", updatedSettings.Language);
            Assert.AreEqual(user.FirstName + " " + user.LastName, updatedSettings.Name);
            Assert.AreEqual(user.Email, updatedSettings.Email);

            _mockSettingsRepository.Verify(repo => repo.Update(It.IsAny<Setting>()), Times.Once);
            _mockLogger.Verify(logger => logger.LogInfo(It.IsAny<string>()), Times.Once);
        }
    }
}