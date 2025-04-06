using ErrorOr;
using Fliq.Application.Authentication.Queries.SetupData;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Entities.Profile;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fliq.Application.Tests.Authentication.Queries.SetupData
{
    [TestClass]
    public class GetProfileSetupDataQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<GetProfileSetupDataQueryHandler>> _mockLogger;
        private readonly GetProfileSetupDataQueryHandler _handler;

        public GetProfileSetupDataQueryHandlerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<GetProfileSetupDataQueryHandler>>();
            _handler = new GetProfileSetupDataQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Handle_WhenDataExists_ReturnsSuccessResult()
        {
            // Arrange
            var expectedResponse = new ProfileDataTablesResponse
            {
                Occupations = new List<Occupation> { new Occupation() },
                Religions = new List<Religion> { new Religion() },
                Ethnicities = new List<Ethnicity> { new Ethnicity() },
                EducationStatuses = new List<EducationStatus> { new EducationStatus() }
            };

            _mockUserRepository.Setup(x => x.GetAllProfileSetupData(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var query = new GetProfileSetupDataQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.AreEqual(expectedResponse, result.Value);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching profile setup data")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenRepositoryReturnsError_ReturnsErrorAndLogsWarning()
        {
            // Arrange
            var expectedError = Error.NotFound("Data not found");
            _mockUserRepository.Setup(x => x.GetAllProfileSetupData(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedError);

            var query = new GetProfileSetupDataQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(expectedError, result.Errors.First());
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch profile setup data")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Handle_WhenExceptionThrown_ReturnsUnexpectedErrorAndLogsError()
        {
            // Arrange
            var exception = new Exception("Test exception");
            _mockUserRepository.Setup(x => x.GetAllProfileSetupData(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var query = new GetProfileSetupDataQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(ErrorType.Unexpected, result.FirstError.Type);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }


    }
}