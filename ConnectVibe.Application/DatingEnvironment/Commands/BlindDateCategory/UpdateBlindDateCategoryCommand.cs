
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDateCategory;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDateCategory
{
    public record UpdateBlindDateCategoryCommand(int Id, string? CategoryName, string? Description) : IRequest<ErrorOr<AddBlindDateCategoryResult>>;

    public class UpdateBlindDateCategoryCommandHandler
    : IRequestHandler<UpdateBlindDateCategoryCommand, ErrorOr<AddBlindDateCategoryResult>>
    {
        private readonly IBlindDateCategoryRepository _blindDateCategoryRepository;
        private readonly ILoggerManager _loggerManager;

        public UpdateBlindDateCategoryCommandHandler(IBlindDateCategoryRepository blindDateCategoryRepository, ILoggerManager loggerManager)
        {
            _blindDateCategoryRepository = blindDateCategoryRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<AddBlindDateCategoryResult>> Handle(UpdateBlindDateCategoryCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting update process for blind date category ID: {request.Id}");

            var category = await _blindDateCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                _loggerManager.LogWarn($"Blind date category ID: {request.Id} not found.");
                return Errors.Dating.BlindDateCategoryNotFound;
            }

            // Update fields
            category.CategoryName = request.CategoryName ?? category.CategoryName;
            category.Description = request.Description ?? category.Description;

            await _blindDateCategoryRepository.UpdateAsync(category);
            _loggerManager.LogInfo($"Successfully updated blind date category ID: {category.Id}");

            return new AddBlindDateCategoryResult(category.Id, category.CategoryName);
        }
    }

}
