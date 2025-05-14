using ErrorOr;
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
        private CreateProfileCommandHandler? _handler;
        private Mock<IProfileRepository>? _profileRepositoryMock;
        private Mock<IUserRepository>? _userRepositoryMock;
        private Mock<ILocationService>? _locationServiceMock;
        private Mock<IHttpContextAccessor>? _httpContextAccessorMock;
        private Mock<ClaimsPrincipal>? _claimsPrincipalMock;
        private Mock<IPromptQuestionRepository>? _promptQuestionRepositoryMock;
        private Mock<IPromptCategoryRepository>? _promptCategoryRepositoryMock;
        private Mock<ILoggerManager>? _loggerManagerMock;
        private Mock<IMediaServices>? _mediaServicesMock;
        private Mock<IDocumentUploadService>? _documentUploadServiceMock;
        private Mock<IBusinessIdentificationDocumentRepository>? _businessIdentificationDocumentRepositoryMock;
        private Mock<IBusinessIdentificationDocumentTypeRepository>? _businessIdentificationDocumentTypeRepositoryMock;
        private Mock<IMapper>? _mapperMock; 

        [TestInitialize]
        public void Setup()
        {
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            _promptQuestionRepositoryMock = new Mock<IPromptQuestionRepository>();
            _promptCategoryRepositoryMock = new Mock<IPromptCategoryRepository>();
            _loggerManagerMock = new Mock<ILoggerManager>();
            _mediaServicesMock = new Mock<IMediaServices>();
            _documentUploadServiceMock = new Mock<IDocumentUploadService>();
            _businessIdentificationDocumentRepositoryMock = new Mock<IBusinessIdentificationDocumentRepository>();
            _businessIdentificationDocumentTypeRepositoryMock = new Mock<IBusinessIdentificationDocumentTypeRepository>();
            _mapperMock = new Mock<IMapper>(); // Initialize IMapper mock

            _httpContextAccessorMock.Setup(x => x.HttpContext.User)
                .Returns(_claimsPrincipalMock.Object);

            // Setup mapper to handle mapping from CreateProfileCommand to UserProfile
            _mapperMock.Setup(m => m.Map(It.IsAny<CreateProfileCommand>(), It.IsAny<UserProfile>()))
                .Returns<CreateProfileCommand, UserProfile>((cmd, profile) =>
                {
                    profile.GenderId = cmd.GenderId;
                    profile.WantKidsId = cmd.WantKidsId;
                    profile.HaveKidsId = cmd.HaveKidsId;
                    profile.DOB = cmd.DOB ?? DateTime.MinValue;
                    profile.ProfileDescription = cmd.ProfileDescription;
                    return profile;
                });

            // Setup mapper for LocationQueryResponse to LocationDetail
            _mapperMock.Setup(m => m.Map<LocationDetail>(It.IsAny<LocationQueryResponse>()))
                .Returns<LocationQueryResponse>(resp => new LocationDetail());

            _handler = new CreateProfileCommandHandler(
                _mapperMock.Object, // Pass IMapper mock
                _profileRepositoryMock.Object,
                _userRepositoryMock.Object,
                _locationServiceMock.Object,
                _loggerManagerMock.Object,
                _promptQuestionRepositoryMock.Object,
                _promptCategoryRepositoryMock.Object,
                _mediaServicesMock.Object,
                _documentUploadServiceMock.Object,
                _businessIdentificationDocumentRepositoryMock.Object,
                _businessIdentificationDocumentTypeRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_UserNotFound_ReturnsProfileNotFoundError()
        {
            // Arrange
            var command = new CreateProfileCommand { UserId = 1 };

            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns((User?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

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
                GenderId = 1,
                ProfileDescription = "Test Description",
                Photos = [new ProfilePhotoMapped { ImageFile = CreateMockFormFile(), Caption = "Test Photo" }],
                Location = new Location { Lat = 51.5074, Lng = -0.1278 }
            };

            var user = new User { Id = 1 };
            var locationResponse = new LocationQueryResponse { Status = "OK" };

            _userRepositoryMock?.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _locationServiceMock.Setup(service => service.GetAddressFromCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(locationResponse);

            _mediaServicesMock.Setup(service => service.UploadImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("image.jpeg");

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

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
            var result = await _handler!.Handle(command, CancellationToken.None);

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
            var result = await _handler!.Handle(command, CancellationToken.None);

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
            var result = await _handler!.Handle(command, CancellationToken.None);

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
                .Returns(new PromptCategory { CategoryName = "Category Name", Id = 1 });

            _promptQuestionRepositoryMock.Setup(repo => repo.GetQuestionByIdAsync(It.IsAny<int>()))
                .Returns((PromptQuestion?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

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
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Prompts.AnswerNotProvided, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_ExistingProfile_ReturnsDuplicateProfileError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                DOB = DateTime.Now.AddYears(-25),
                GenderId = 1
            };
            var user = new User { Id = 1 };
            var existingProfile = new UserProfile { UserId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);
            _profileRepositoryMock.Setup(repo => repo.GetProfileByUserId(It.IsAny<int>()))
                .Returns(existingProfile);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Profile.DuplicateProfile, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_BusinessDocumentsUploadSuccessful_CreatesDocumentSuccessfully()
        {
            // Arrange
            var frontDoc = CreateMockFormFile("business_front.pdf", "application/pdf");
            var backDoc = CreateMockFormFile("business_back.jpg", "image/jpeg");
            var command = new CreateProfileCommand
            {
                UserId = 1,
                BusinessIdentificationDocuments = new BusinessIdentificationDocumentMapped
                {
                    BusinessIdentificationDocumentTypeId = 1,
                    BusinessIdentificationDocumentFront = frontDoc,
                    BusinessIdentificationDocumentBack = backDoc
                }
            };
            var user = new User { Id = 1 };
            var uploadResult = new DocumentUploadResult
            {
                Success = true,
                FrontDocumentUrl = "front_document_url.pdf",
                BackDocumentUrl = "back_document_url.jpg"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);
            _businessIdentificationDocumentTypeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new BusinessIdentificationDocumentType { Id = 1, IsDeleted = false });
            _documentUploadServiceMock.Setup(service => service.UploadDocumentsAsync(
                    It.IsAny<int>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsError);
            _businessIdentificationDocumentRepositoryMock.Verify(repo => repo.Add(It.IsAny<BusinessIdentificationDocument>()), Times.Once);
            _businessIdentificationDocumentRepositoryMock.Verify(repo => repo.Add(It.Is<BusinessIdentificationDocument>(
                doc => doc.BusinessIdentificationDocumentTypeId == 1 &&
                       doc.FrontDocumentUrl == "front_document_url.pdf" &&
                       doc.BackDocumentUrl == "back_document_url.jpg" &&
                       doc.IsVerified == false)), Times.Once);
        }

        [TestMethod]
        public async Task Handle_InvalidBusinessDocumentType_ReturnsInvalidDocumentTypeError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                BusinessIdentificationDocuments = new BusinessIdentificationDocumentMapped
                {
                    BusinessIdentificationDocumentTypeId = 999,
                    BusinessIdentificationDocumentFront = CreateMockFormFile(),
                    BusinessIdentificationDocumentBack = CreateMockFormFile()
                }
            };

            var user = new User { Id = 1 };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);

            _businessIdentificationDocumentTypeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((BusinessIdentificationDocumentType?)null);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Document.InvalidDocumentType, result.FirstError);
        }

        [TestMethod]
        public async Task Handle_BusinessDocumentUploadFails_ReturnsInvalidDocumentError()
        {
            // Arrange
            var command = new CreateProfileCommand
            {
                UserId = 1,
                BusinessIdentificationDocuments = new BusinessIdentificationDocumentMapped
                {
                    BusinessIdentificationDocumentTypeId = 1,
                    BusinessIdentificationDocumentFront = CreateMockFormFile(),
                    BusinessIdentificationDocumentBack = CreateMockFormFile()
                }
            };
            var user = new User { Id = 1 };
            var uploadResult = new DocumentUploadResult
            {
                Success = false,
                ErrorMessage = "Failed to upload documents"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<int>()))
                .Returns(user);
            _businessIdentificationDocumentTypeRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new BusinessIdentificationDocumentType { Id = 1, IsDeleted = false });
            _documentUploadServiceMock.Setup(service => service.UploadDocumentsAsync(
                    It.IsAny<int>(),
                    It.IsAny<IFormFile>(),
                    It.IsAny<IFormFile>()))
                .ReturnsAsync(uploadResult);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(Errors.Document.InvalidDocument, result.FirstError);
        }

        private IFormFile CreateMockFormFile(string fileName = "test.jpg", string contentType = "image/jpeg")
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns(contentType);
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