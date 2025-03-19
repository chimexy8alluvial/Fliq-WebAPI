using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class CreateEventCommandHandlerTests
    {
        private Mock<IMapper>? _mapperMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IMediaServices>? _mediaServicesMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILocationService>? _locationServiceMock;
        private Mock<IEventService>? _eventServiceMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<IMediator>? _mediatorMock;
        private Mock<ICurrencyRepository>? _currencyRepositoryMock;

        private CreateEventCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediaServicesMock = new Mock<IMediaServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _eventServiceMock = new Mock<IEventService>();
            _emailServiceMock = new Mock<IEmailService>();
            _mediatorMock = new Mock<IMediator>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();

            _handler = new CreateEventCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _mediaServicesMock.Object,
                _eventRepositoryMock.Object,
                _locationServiceMock.Object,
                _eventServiceMock.Object,
                _emailServiceMock.Object,
                _mediatorMock.Object,
                _currencyRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotVerified_ReturnsDuplicateEmailError()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                UserId = 1,
                EventType = EventType.Physical
            };

            var user = new User { IsDocumentVerified = false };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.DuplicateEmail, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_MapsAndSavesEvent()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                UserId = 1,
                EventType = EventType.Live,
                EventTitle = "Test Event",
                Location = new Location { Lat = 40.7128, Lng = -74.0060 },

                MediaDocuments = new List<EventMediaMapped>
                {
                    new EventMediaMapped { DocFile = CreateMockFormFile(), Title = "Valid File" }
                }
            };

            var user = new User { IsDocumentVerified = true, Id = 1 };
            var newEvent = new Events { EventTitle = "Test Event", UserId = 1, EventType = EventType.Live, Media = new List<EventMedia>() };

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

            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _mapperMock?.Setup(mapper => mapper.Map<Events>(command)).Returns(newEvent);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
               .ReturnsAsync(locationResponse);
            _mediaServicesMock?.Setup(service => service.UploadMediaAsync(It.IsAny<IFormFile>(), "Event Documents")).ReturnsAsync("image.jpeg");
            _mediaServicesMock?.Setup(service => service.UploadMediaAsync(It.IsAny<IFormFile>(), "Event Documents"))
              .ReturnsAsync("image.jpeg");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _eventRepositoryMock?.Verify(repo => repo.Add(It.IsAny<Events>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidMediaDocument_ReturnsInvalidDocumentError()
        {
            // Arrange
            var command = new CreateEventCommand
            {
                UserId = 1,
                EventType = EventType.Live,
                MediaDocuments = new List<EventMediaMapped>
                {
                    new EventMediaMapped { DocFile = CreateMockFormFile(), Title = "Invalid File" }
                }
            };

            var image = CreateMockFormFile();
            var user = new User { IsDocumentVerified = true };
            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _mediaServicesMock?.Setup(service => service.UploadMediaAsync(image, "Event Documents")).ReturnsAsync((string)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Document.InvalidDocument, result.FirstError);
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