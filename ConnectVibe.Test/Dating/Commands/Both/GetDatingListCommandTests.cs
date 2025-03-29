
//using Fliq.Application.Common.Interfaces.Persistence;
//using Fliq.Application.Common.Interfaces.Services;
//using Fliq.Application.DatingEnvironment.Commands;
//using Fliq.Contracts.Dating;
//using Fliq.Domain.Entities.Event.Enums;
//using Fliq.Infrastructure.Persistence.Repositories;
//using Fliq.Domain.Common.Errors;
//using Moq;
//using System;

//namespace Fliq.Test.Dating.Commands.Both
//{
//    [TestClass]
//    public class GetDatingListCommandTests
//    {
//        private Mock<ILoggerManager>? _mockLoggerManager;
//        private Mock<ISpeedDatingEventRepository>? _mockSpeedDatingEventRepository;
//        private Mock<IBlindDateRepository>? _mockBlindDateRepository;
//        private GetDatingListCommandHandler? _handler;

//        [TestInitialize]
//        public void Setup()
//        {
//            _mockLoggerManager = new Mock<ILoggerManager>();
//            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
//            _mockSpeedDatingEventRepository = new Mock<ISpeedDatingEventRepository>();

//            _handler = new GetDatingListCommandHandler(
//                _mockLoggerManager.Object,
//                _mockSpeedDatingEventRepository.Object,
//                _mockBlindDateRepository.Object
//            );

//        }

//        [TestMethod]
//        public async Task Handle_ValidRequest_ReturnsDatingEventsSuccessfully()
//        {
//            // Arrange
//            var command = new GetDatingListCommand(
//                Title: "Test",
//                Type: null,
//                Duration: null,
//                SubscriptionType: null,
//                DateCreated: new DateTime(2025, 3, 27),
//                CreatedBy: "user1",
//                Page: 1,
//                PageSize: 2
//            );

//            var blindDates = new List<DatingListItem>
//            {
//                new DatingListItem { Id = 1, Title = "Blind Date 1", Type = DatingType.BlindDating, DateCreated = new DateTime(2025, 3, 27), CreatedBy = "user1", SubscriptionType = "Premium User", Duration = TimeSpan.FromSeconds(1) }
//            };
//            var speedDates = new List<DatingListItem>
//            {
//                new DatingListItem { Id = 2, Title = "Speed Date 1", Type = DatingType.SpeedDating, DateCreated = new DateTime(2025, 3, 27), CreatedBy = "user1", SubscriptionType = "Premium User", Duration = TimeSpan.FromSeconds(1) }
//            };

//            _mockBlindDateRepository?
//                .Setup(repo => repo.GetAllFilteredListAsync("Test", null, null, null, It.IsAny<DateTime?>(), "user1", 1, 2))
//                .ReturnsAsync((blindDates, 1));

//            _mockSpeedDatingEventRepository?
//                .Setup(repo => repo.GetAllFilteredListAsync("Test", null, null, null, It.IsAny<DateTime?>(), "user1", 1, 2))
//                .ReturnsAsync((speedDates, 1));

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.IsFalse(result.IsError);
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(2, result.Value.List.Count); 
//            Assert.AreEqual(2, result.Value.TotalCount);   
//            Assert.AreEqual("Blind Date 1", result.Value.List[0].Title);
//            Assert.AreEqual("Speed Date 1", result.Value.List[1].Title);

//            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
//            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
//            _mockLoggerManager?.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Retrieved 2 events out of 2 total matching filters"))), Times.Once);
//        }

//        [TestMethod]
//        public async Task Handle_InvalidPage_ReturnsError()
//        {
//            // Arrange
//            var command = new GetDatingListCommand(
//                Title: null,
//                Type: null,
//                Duration: null,
//                SubscriptionType: null,
//                DateCreated: null,
//                CreatedBy: null,
//                Page: 0, 
//                PageSize: 10
//            );

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.IsTrue(result.IsError);
//            //Assert.AreEqual(Errors.Dating.InvalidPagination("Page must be greater than 0").Description, result.Errors.First().Description);

//            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
//            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
//            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("Invalid page number provided"))), Times.Once);
//        }

//        [TestMethod]
//        public async Task Handle_NoEventsFound_ReturnsError()
//        {
//            // Arrange
//            var command = new GetDatingListCommand(
//                Title: "NonExistent",
//                Type: null,
//                Duration: null,
//                SubscriptionType: null,
//                DateCreated: new DateTime(2025, 3, 27),
//                CreatedBy: "user1",
//                Page: 1,
//                PageSize: 10
//            );

//            _mockBlindDateRepository?
//                .Setup(repo => repo.GetAllFilteredListAsync("NonExistent", null, null, null, It.IsAny<DateTime?>(), "user1", 1, 10))
//                .ReturnsAsync((new List<DatingListItem>(), 0));

//            _mockSpeedDatingEventRepository?
//                .Setup(repo => repo.GetAllFilteredListAsync("NonExistent", null, null, null, It.IsAny<DateTime?>(), "user1", 1, 10))
//                .ReturnsAsync((new List<DatingListItem>(), 0)); 

//            // Act
//            var result = await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.IsTrue(result.IsError);
//            Assert.AreEqual(Errors.Dating.NoEventsFound.Description, result.Errors.First().Description);

//            _mockBlindDateRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
//            _mockSpeedDatingEventRepository?.Verify(repo => repo.GetAllFilteredListAsync(It.IsAny<string>(), It.IsAny<DatingType?>(), It.IsAny<TimeSpan>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
//            _mockLoggerManager?.Verify(logger => logger.LogError(It.Is<string>(msg => msg.Contains("No dating events found matching the provided filters"))), Times.Once);
//        }
//    }
//}




////using Fliq.Application.Common.Interfaces.Persistence;
////using Fliq.Application.Common.Interfaces.Services.LocationServices;
////using Fliq.Application.Common.Interfaces.Services.MeidaServices;
////using Fliq.Application.Common.Interfaces.Services;
////using Fliq.Application.DatingEnvironment.Commands.BlindDates;
////using Fliq.Application.DatingEnvironment.Common;
////using MapsterMapper;
////using Moq;
////using Microsoft.AspNetCore.Http;
////using System.Text;
////using Fliq.Domain.Entities.Profile;
////using Fliq.Application.Common.Models;
////using Fliq.Domain.Entities.DatingEnvironment.BlindDates;

////namespace Fliq.Test.Dating.Commands.BlindDating
////{
////    [TestClass]
////    public class CreateBlindDateCommandHandlerTests
////    {
////        private Mock<IBlindDateRepository> _mockBlindDateRepository;
////        private Mock<IBlindDateCategoryRepository> _mockBlindDateCategoryRepository;
////        private Mock<ILoggerManager> _mockLoggerManager;
////        private Mock<IMapper> _mockMapper;
////        private Mock<ILocationService> _mockLocationService;
////        private Mock<IMediaServices> _mockMediaServices;
////        private Mock<IBlindDateParticipantRepository> _mockBlindDateParticipantRepository;
////        private AddBlindDateCommandHandler _handler;

////        [TestInitialize]
////        public void Setup()
////        {
////            _mockBlindDateRepository = new Mock<IBlindDateRepository>();
////            _mockBlindDateCategoryRepository = new Mock<IBlindDateCategoryRepository>();
////            _mockLoggerManager = new Mock<ILoggerManager>();
////            _mockMapper = new Mock<IMapper>();
////            _mockLocationService = new Mock<ILocationService>();
////            _mockMediaServices = new Mock<IMediaServices>();
////            _mockBlindDateParticipantRepository = new Mock<IBlindDateParticipantRepository>();

////            _handler = new AddBlindDateCommandHandler(
////                _mockBlindDateRepository.Object,
////                _mockLoggerManager.Object,
////                _mockBlindDateCategoryRepository.Object,
////                _mockMapper.Object,
////                _mockLocationService.Object,
////                _mockMediaServices.Object,
////                _mockBlindDateParticipantRepository.Object
////            );
////        }

////        [TestMethod]
////        public async Task Handle_ValidRequest_CreatesBlindDateSuccessfully()
////        {
////            // Arrange
////            var location = new Location { Lat = 51.5074, Lng = -0.1278, IsVisible = true };
////            var locationDetail = new LocationDetail { Results = [], Status = "", Location = location };
////            var command = new CreateBlindDateCommand(
////                Title: "Speed Dating Night",
////                CategoryId: 1,
////                Location: location,
////                  LocationDetail: locationDetail,
////                BlindDateImage: new DatePhotoMapped(CreateMockFormFile()),
////                IsOneOnOne: false,
////                NumberOfParticipants: 10,
////                IsRecordingEnabled: true,
////                CreatedByUserId: 1001,
////                StartDateTime: DateTime.UtcNow.AddDays(5), // Future date
////                 SessionStartTime: null,
////                 SessionEndTime: null

////            );

////            var category = new BlindDateCategory { Id = 1, CategoryName = "Casual" };
////            var blindDateEntity = new BlindDate { Id = 1, Title = "Speed Dating Night" };
////            var locationResponse = new LocationQueryResponse
////            {
////                PlusCode = new Application.Common.Models.PlusCode { CompoundCode = "FakeCode", GlobalCode = "GlobalCode123" },
////                Results = new List<Result>
////                {
////                    new Result
////                    {
////                        FormattedAddress = "123 Fake Street, Faketown, Fakestate",
////                        Geometry = new Application.Common.Models.Geometry
////                        {
////                            Location = new Application.Common.Models.Locationn
////                            {
////                                Lat = 40.7128,
////                                Lng = -74.0060
////                            }
////                        }
////                    }
////                },
////                Status = "OK"
////            };
////            _mockBlindDateCategoryRepository
////                .Setup(repo => repo.GetByIdAsync(command.CategoryId))
////                .ReturnsAsync(category);

////            _mockMapper
////                .Setup(m => m.Map<BlindDate>(command))
////                .Returns(blindDateEntity);

////            _mockLocationService
////                .Setup(s => s.GetAddressFromCoordinatesAsync(command.Location.Lat, command.Location.Lng))
////                .ReturnsAsync(locationResponse);

////            _mockMediaServices
////                .Setup(m => m.UploadImageAsync(It.IsAny<IFormFile>()))
////                .ReturnsAsync("image.jpeg");

////            // Act
////            var result = await _handler.Handle(command, CancellationToken.None);

////            // Assert
////            Assert.IsFalse(result.IsError);
////            Assert.IsNotNull(result.Value);
////            Assert.AreEqual(1, result.Value.Id);
////            Assert.AreEqual(command.Title, result.Value.Title);

////            _mockBlindDateRepository.Verify(repo => repo.AddAsync(It.IsAny<BlindDate>()), Times.Once);
////            _mockBlindDateParticipantRepository.Verify(repo => repo.AddAsync(It.IsAny<BlindDateParticipant>()), Times.Once);
////            _mockLoggerManager.Verify(logger => logger.LogInfo(It.Is<string>(msg => msg.Contains("Successfully created Blind Date"))), Times.Once);
////        }

////        [TestMethod]
////        public async Task Handle_InvalidRequest_ReturnsError()
////        {
////            // Arrange - Missing required fields
////            var location = new Location { Lat = 0, Lng = 0, IsVisible = false }; // Invalid location
////            var locationDetail = new LocationDetail { Results = [], Status = "", Location = location };

////            var command = new CreateBlindDateCommand(
////                Title: "", // Empty title (invalid)
////                CategoryId: 0, // Invalid category ID
////                Location: location,
////                LocationDetail: locationDetail,
////                BlindDateImage: null, // No image provided
////                IsOneOnOne: false,
////                NumberOfParticipants: -5, // Invalid negative participants
////                IsRecordingEnabled: true,
////                CreatedByUserId: 0, // Invalid user ID
////                StartDateTime: DateTime.UtcNow.AddDays(-1), // Past date (invalid)
////                SessionStartTime: null,
////                SessionEndTime: null
////            );

////            _mockBlindDateCategoryRepository
////                .Setup(repo => repo.GetByIdAsync(command.CategoryId))
////                .ReturnsAsync((BlindDateCategory)null); // Category not found

////            _mockLocationService
////                .Setup(s => s.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
////                .ReturnsAsync((LocationQueryResponse)null); // Location not found

////            // Act
////            var result = await _handler.Handle(command, CancellationToken.None);

////            // Assert
////            Assert.IsTrue(result.IsError);
////            Assert.IsNull(result.Value);
////            Assert.IsTrue(result.Errors.Any());

////            _mockBlindDateRepository.Verify(repo => repo.AddAsync(It.IsAny<BlindDate>()), Times.Never);
////            _mockLoggerManager.Verify(logger => logger.LogWarn(It.IsAny<string>()), Times.AtLeastOnce);
////        }



////        private IFormFile CreateMockFormFile()
////        {
////            var fileMock = new Mock<IFormFile>();

////            // Mock file content
////            var content = "Fake file content";
////            var fileName = "test.jpg";
////            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

////            // Set up the stream for reading
////            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);

////            // Setup file properties
////            fileMock.Setup(_ => _.FileName).Returns(fileName);
////            fileMock.Setup(_ => _.Length).Returns(ms.Length);
////            fileMock.Setup(_ => _.ContentType).Returns("image/jpeg");
////            fileMock.Setup(_ => _.Name).Returns("file");
////            fileMock.Setup(_ => _.Headers).Returns(new HeaderDictionary()); // Headers mock

////            // Mock the CopyTo method (synchronous)
////            fileMock.Setup(_ => _.CopyTo(It.IsAny<Stream>()))
////                .Callback<Stream>(stream => ms.CopyTo(stream));2

////            // Mock the CopyToAsync method (asynchronous)
////            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
////                .Returns<Stream, CancellationToken>((stream, token) => ms.CopyToAsync(stream, token));

////            return fileMock.Object;
////        }
////    }
////}

