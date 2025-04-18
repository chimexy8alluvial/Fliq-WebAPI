using FluentValidation;

namespace Fliq.Application.BusinessDocumentType.Common
{
    public record BusinessDocumentTypeRequest
    {
        public string Name { get; init; } = string.Empty;
        public bool HasFrontAndBack { get; init; }

        public static object Adapt<T>()
        {
            throw new NotImplementedException();
        }
    }

    public class BusinessDocumentTypeRequestValidator : AbstractValidator<BusinessDocumentTypeRequest>
    {
        public BusinessDocumentTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Document type name is required.")
                .MaximumLength(100)
                .WithMessage("Document type name cannot exceed 100 characters.");
        }
    }
}
