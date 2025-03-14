
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

            // Trim inputs to remove accidental whitespace changes
            var newCategoryName = request.CategoryName?.Trim();
            var newDescription = request.Description?.Trim();

            // Check if any update is needed
            bool isUpdated = false;

            if (!string.IsNullOrWhiteSpace(newCategoryName) && newCategoryName != category.CategoryName)
            {
                category.CategoryName = newCategoryName;
                isUpdated = true;
            }

            if (!string.IsNullOrWhiteSpace(newDescription) && newDescription != category.Description)
            {
                category.Description = newDescription;
                isUpdated = true;
            }

            // If nothing changed, return without updating the DB
            if (!isUpdated)
            {
                _loggerManager.LogInfo($"No changes detected for blind date category ID: {category.Id}");
                return new AddBlindDateCategoryResult(category.Id, category.CategoryName);
            }

            await _blindDateCategoryRepository.UpdateAsync(category);
            _loggerManager.LogInfo($"Successfully updated blind date category ID: {category.Id}");

            return new AddBlindDateCategoryResult(category.Id, category.CategoryName);
        }
    }

}
