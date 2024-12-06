using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Common.Models;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Application.Event.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Profile;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Fliq.Test.Event.Commands
{
    [TestClass]
    public class UpdateEventCommandHandlerTests
    {
        private Mock<IMapper>? _mapperMock;
        private Mock<ILoggerManager>? _loggerMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<IMediaServices>? _mediaServicesMock;
        private Mock<IEventRepository>? _eventRepositoryMock;
        private Mock<ILocationService>? _locationServiceMock;

        private UpdateEventCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILoggerManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediaServicesMock = new Mock<IMediaServices>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _locationServiceMock = new Mock<ILocationService>();

            _handler = new UpdateEventCommandHandler(
                _mapperMock.Object,
                _loggerMock.Object,
                _userRepositoryMock.Object,
                _mediaServicesMock.Object,
                _eventRepositoryMock.Object,
                _locationServiceMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
        {
            // Arrange
            var command = new UpdateEventCommand
            {
                EventId = 1,
                UserId = 1
            };

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns((Events)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Event.EventNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
        {
            // Arrange
            var command = new UpdateEventCommand
            {
                EventId = 1,
                UserId = 1
            };

            var existingEvent = new Events();
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(existingEvent);
            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.User.UserNotFound, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ValidUpdate_UpdatesEvent()
        {
            // Arrange
            var command = new UpdateEventCommand
            {
                EventId = 1,
                UserId = 1,
                EventTitle = "Updated Title",

                MediaDocuments = new List<EventMediaMapped>
                {
                    new EventMediaMapped { DocFile = CreateMockFormFile() , Title = "New Media" }
                }
            };

            var existingEvent = new Events()
            {
                Media = new List<EventMedia>
                {
                    new EventMedia{MediaUrl = "Media Url", Id = 1, Title = "Old Media"}
                }
            };

            var user = new User();

            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(existingEvent);
            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _mediaServicesMock?.Setup(service => service.UploadMediaAsync(It.IsAny<IFormFile>(), "Event Documents")).ReturnsAsync("image.jpeg");
            _mediaServicesMock?.Setup(service => service.UploadMediaAsync(It.IsAny<IFormFile>(), "Event Documents"))
              .ReturnsAsync("image.jpeg");
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _eventRepositoryMock?.Verify(repo => repo.Update(It.IsAny<Events>()), Times.Once);
            Assert.AreEqual("Updated Title", existingEvent.EventTitle);
            Assert.AreEqual(1, existingEvent.Media.Count);
        }

        [TestMethod]
        public async Task Handle_ValidLocation_UpdatesEventLocation()
        {
            // Arrange
            var command = new UpdateEventCommand
            {
                EventId = 1,
                UserId = 1,
                Location = new Location { Lat = 10.0, Lng = 20.0, IsVisible = true }
            };

            var existingEvent = new Events();
            var user = new User();
            var locationDetail = new LocationDetail();

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
            _eventRepositoryMock?.Setup(repo => repo.GetEventById(It.IsAny<int>())).Returns(existingEvent);
            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns(user);
            _locationServiceMock?.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
     .ReturnsAsync(locationResponse);
            _mapperMock?.Setup(mapper => mapper.Map<LocationDetail>(locationDetail)).Returns(locationDetail);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            Assert.IsNotNull(existingEvent.Location);
            Assert.AreEqual(command.Location.Lat, existingEvent.Location.Lat);
            Assert.AreEqual(command.Location.Lng, existingEvent.Location.Lng);
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