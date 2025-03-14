using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Profile.Queries.Get;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fliq.Test.Profile.Queries.Get
{
    [TestClass]
    public class GetProfileQueryHandlerTests
    {
        private Mock<IProfileRepository>? _mockProfileRepository;
        private Mock<IUserRepository>? _mockUserRepository;
        private Mock<IHttpContextAccessor>? _mockHttpContextAccessor;
        private Mock<ILoggerManager>? _mockLoggerManager;
        private GetProfileQueryHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockProfileRepository = new Mock<IProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLoggerManager = new Mock<ILoggerManager>();

            _handler = new GetProfileQueryHandler(
                _mockProfileRepository.Object,
                _mockUserRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockLoggerManager.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var query = new GetProfileQuery(999);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Profile.ProfileNotFound));
        }

        [TestMethod]
        public async Task Handle_ProfileNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };

            var query = new GetProfileQuery(user.Id);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockProfileRepository?
                .Setup(repo => repo.GetProfileByUserId(It.IsAny<int>()))
                .Returns((UserProfile?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.IsTrue(result.Errors.Contains(Errors.Profile.ProfileNotFound));
        }

        [TestMethod]
        public async Task Handle_ValidQuery_ReturnsProfileResult()
        {
            // Arrange
            var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var location = new Location { Lat = 51.5074, Lng = -0.1278, IsVisible = true };
            var profile = new UserProfile
            {
                DOB = DateTime.Now.AddYears(-25),
                Gender = new Gender { GenderType = GenderType.Male },
                ProfileDescription = "I am a software engineer who admires hardworking women in tech description",
                Location = location,
                UserId = user.Id
            };

            var query = new GetProfileQuery(user.Id);

            _mockUserRepository?
                .Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _mockProfileRepository?
                .Setup(repo => repo.GetProfileByUserId(It.IsAny<int>()))
                .Returns(profile);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            var returnedProfile = result.Value;

            Assert.AreEqual(profile.Id, returnedProfile.Profile.Id);
            Assert.AreEqual(user.Id, returnedProfile.Profile.UserId);
            Assert.AreEqual(profile.Gender, returnedProfile.Profile.Gender);
            Assert.AreEqual(profile.ProfileDescription, returnedProfile.Profile.ProfileDescription);
            Assert.AreEqual(profile.Location, returnedProfile.Profile.Location);
        }
    }
}