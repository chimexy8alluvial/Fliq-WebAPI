//using FluentValidation;
//using Microsoft.AspNetCore.Http;
//using Fliq.Application.Authentication.Business.Command.Register;

//namespace Fliq.Application.Authentication.Commands.RegisterBusiness
//{
//    public class RegisterBusinessCommandValidator : AbstractValidator<RegisterBusinessCommand>
//    {
//        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
//        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB in bytes

//        public RegisterBusinessCommandValidator()
//        {
//            // AccountType
//            RuleFor(x => x.AccountType)
//                .NotNull()
//                .WithMessage("Account type is required.")
//                .IsInEnum()
//                .WithMessage("Invalid account type.");

//            // Email
//            RuleFor(x => x.Email)
//                .NotEmpty()
//                .WithMessage("Email is required.")
//                .EmailAddress()
//                .WithMessage("Invalid email format.");

//            // Password
//            RuleFor(x => x.Password)
//                .NotEmpty()
//                .WithMessage("Password is required.")
//                .MinimumLength(8)
//                .WithMessage("Password must be at least 8 characters long.")
//                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
//                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
//                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
//                .Matches(@"[!@#$%^&*]").WithMessage("Password must contain at least one special character.");

//            // BusinessName
//            RuleFor(x => x.BusinessName)
//                .NotEmpty()
//                .WithMessage("Business name is required.")
//                .MaximumLength(100)
//                .WithMessage("Business name cannot exceed 100 characters.");

//            // BusinessType
//            RuleFor(x => x.BusinessType)
//                .NotEmpty()
//                .WithMessage("Business type is required.")
//                .MaximumLength(50)
//                .WithMessage("Business type cannot exceed 50 characters.");

//            // Address
//            RuleFor(x => x.Address)
//                .NotEmpty()
//                .WithMessage("Address is required.")
//                .MaximumLength(200)
//                .WithMessage("Address cannot exceed 200 characters.");

//            // PhoneNumber
//            RuleFor(x => x.PhoneNumber)
//                .NotEmpty()
//                .WithMessage("Phone number is required.")
//                .MaximumLength(11)
//                .WithMessage("Phone number cannot exceed 11 characters.");

//            // Country
//            RuleFor(x => x.Country)
//                .NotNull()
//                .WithMessage("Country is required.")
//                .IsInEnum()
//                .WithMessage("Invalid country selected.");

//            // Identification
//            RuleFor(x => x.Identification)
//                .NotNull()
//                .WithMessage("Identification type is required.").IsInEnum();

//            // IdentificationFileFront
//            RuleFor(x => x.IdentificationImageFront)
//                .NotNull()
//                .WithMessage("Front image of identification document is required.")
//                .Must(BeAValidImage)
//                .WithMessage("Front identification file must be a valid image (JPEG or PNG).")
//                .Must(BeWithinSizeLimit)
//                .WithMessage($"Front identification file must not exceed {(_maxFileSize / (1024 * 1024))}MB.");

//            // IdentificationFileBack
//            RuleFor(x => x.IdentificationImageBack)
//                .NotNull()
//                .WithMessage("Back image of identification document is required.")
//                .Must(BeAValidImage)
//                .WithMessage("Back identification file must be a valid image (JPEG or PNG).")
//                .Must(BeWithinSizeLimit)
//                .WithMessage($"Back identification file must not exceed {(_maxFileSize / (1024 * 1024))}MB.");
//        }

//        private bool BeAValidImage(IFormFile file)
//        {
//            if (file == null)
//                return false;

//            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//            return _allowedExtensions.Contains(extension);
//        }

//        private bool BeWithinSizeLimit(IFormFile file)
//        {
//            if (file == null)
//                return false;

//            return file.Length <= _maxFileSize;
//        }
//    }
//}