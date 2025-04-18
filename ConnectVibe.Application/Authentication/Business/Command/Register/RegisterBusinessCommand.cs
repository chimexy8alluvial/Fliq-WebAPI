using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Fliq.Application.Common.Security;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Domain.Entities.Settings;
using Fliq.Application.Authentication.Common;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Authentication.Business.Command.Register
{
    public record RegisterBusinessCommand
    (
        //AccountType AccountType,
        string Email,
        string Password,
        string BusinessName,
        string BusinessType,
        string Address,
        string PhoneNumber,
        int DocumentTypeId,
        IFormFile BusinessIdentificationDocumentFront,
        IFormFile? BusinessIdentificationDocumentBack,
        string CompanyBio,
        Language Language,
        ScreenMode Theme
    ) : IRequest<ErrorOr<RegistrationResult>>;

    public class RegisterBusinessCommandHandler : IRequestHandler<RegisterBusinessCommand, ErrorOr<RegistrationResult>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMediaServices _mediaServices;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IDocumentUploadService _documentUploadService;
        private readonly IBusinessDocumentRepository _businessDocumentRepository;
        public RegisterBusinessCommandHandler(IMapper mapper, IEmailService emailService, IOtpService otpService, ILoggerManager logger, ISettingsRepository settingsRepository, IMediaServices mediaServices, IUserRepository userRepository, IDocumentUploadService documentUploadService, IBusinessDocumentRepository businessDocumentRepository)
        {
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
            _settingsRepository = settingsRepository;
            _mediaServices = mediaServices;
            _userRepository = userRepository;
            _documentUploadService = documentUploadService;
            _businessDocumentRepository = businessDocumentRepository;
        }
        public async Task<ErrorOr<RegistrationResult>> Handle(RegisterBusinessCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Registering business with email: {command.Email}");

            // Check for duplicate email
            User? user = _userRepository.GetUserByEmail(command.Email);
            if (user != null && user.IsEmailValidated)
            {
                _logger.LogWarn($"Duplicate email: {command.Email}");
                return Errors.User.DuplicateEmail;
            }

            // Map command to user
            user = user ?? _mapper.Map<User>(command);
            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.Password, user.PasswordSalt);
            user.RoleId = 3;

            // Upload documents
            var uploadResult = await _documentUploadService.UploadDocumentsAsync(
                command.DocumentTypeId,
                command.BusinessIdentificationDocumentFront,
                command.BusinessIdentificationDocumentBack);

            if (!uploadResult.Success)
            {
                _logger.LogError("Document upload failed.");
                return Errors.Document.InvalidDocument;
            }


            user.UserProfile.BusinessIdentificationDocument.Id = command.DocumentTypeId;
            user.UserProfile.BusinessIdentificationDocument.FrontDocumentUrl = uploadResult.FrontDocumentUrl;
            user.UserProfile.BusinessIdentificationDocument.BackDocumentUrl = uploadResult.BackDocumentUrl;

            // Save user
            _userRepository.Add(user);

            // Create business document record
            var businessDocument = new BusinessIdentificationDocument
            {
                UserId = user.Id,
                BusinessDocumentTypeId = command.DocumentTypeId,
                FrontDocumentUrl = uploadResult.FrontDocumentUrl,
                BackDocumentUrl = uploadResult.BackDocumentUrl
            };

            // Add to repository
            _businessDocumentRepository.Add(businessDocument);

            // Save settings
            Setting setting = new Setting
            {
                ScreenMode = command.Theme,
                Language = command.Language,
                User = user,
                UserId = user.Id
            };
            _settingsRepository.Add(setting);

            // Send OTP
            var otp = await _otpService.GetOtpAsync(user.Email, user.Id);
            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");

            _logger.LogInfo($"Business registered successfully: {user.Email}");
            return new RegistrationResult(user, otp);
        }
    }
}


