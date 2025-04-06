using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Queries.GetSettings;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;
using Moq;

namespace Fliq.Test.Settings.Queries.Get
{
    [TestClass]
    public class GetSettingsQueryHandlerTests
    {
        private Mock<ISettingsRepository>? _mockSettingsRepository;
        private Mock<IProfileRepository>? _mockProfileRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private GetSettingsQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockSettingsRepository = new Mock<ISettingsRepository>();
            _mockProfileRepository = new Mock<IProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new GetSettingsQueryHandler(
                _mockSettingsRepository.Object,
                _mockProfileRepository.Object,
                _mockUserRepository.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var query = new GetSettingsQuery(999);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.User.UserNotFound));
        }

        [TestMethod]
        public async Task Handle_SettingsNotFound_ReturnsSettingsNotFoundError()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

            var query = new GetSettingsQuery(user.Id);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockSettingsRepository?
                .Setup(repo => repo.GetSettingByUserId(It.IsAny<int>()))
                .Returns((Setting?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Settings.SettingsNotFound));
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsSettingsResult()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var settings = new Setting
            {
                Id = 1,
                ScreenMode = ScreenMode.Dark,
                RelationAvailability = true,
                ShowMusicAndGameStatus = true,
                Language = Language.English,
                NotificationPreferences = new List<NotificationPreference>(),
                Filter = new Filter()
            };

            var query = new GetSettingsQuery(user.Id);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockSettingsRepository?
                .Setup(repo => repo.GetSettingByUserId(It.IsAny<int>()))
                .Returns(settings);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            var returnedSettings = result.Value;

            Assert.AreEqual(settings.Id, returnedSettings.Id);
            Assert.AreEqual(ScreenMode.Dark, returnedSettings.ScreenMode);
            Assert.AreEqual(user.FirstName + " " + user.LastName, returnedSettings.Name);
            Assert.AreEqual(user.Email, returnedSettings.Email);
            Assert.AreEqual(settings.Language, returnedSettings.Language);
            // Compare the notification preferences list contents
            CollectionAssert.AreEqual(
                settings.NotificationPreferences.Select(x => x.Context).ToList(),
                returnedSettings.NotificationPreferences.Select(x => x.Context).ToList()
            );
            CollectionAssert.AreEqual(
                settings.NotificationPreferences.Select(x => x.PushNotification).ToList(),
                returnedSettings.NotificationPreferences.Select(x => x.PushNotification).ToList()
            );
            CollectionAssert.AreEqual(
                settings.NotificationPreferences.Select(x => x.InAppNotification).ToList(),
                returnedSettings.NotificationPreferences.Select(x => x.InAppNotification).ToList()
            );
        }
    }
}