using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDateCategory;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.DatingEnvironment;
using MediatR;


namespace Fliq.Application.DatingEnvironment.Commands.BlindDateCategory
{
    public record AddBlindDateCategoryCommand(string CategoryName, string? Description) : IRequest<ErrorOr<AddBlindDateCategoryResult>>;

    public class AddBlindDateCategoryCommandHandler : IRequestHandler<AddBlindDateCategoryCommand, ErrorOr<AddBlindDateCategoryResult>>
    {
        private readonly IBlindDateCategoryRepository _blindDateCategoryRepository;
        private readonly ILoggerManager _loggerManager;
        public AddBlindDateCategoryCommandHandler(IBlindDateCategoryRepository blindDateCategoryRepository, ILoggerManager loggerManager)
        {
            _blindDateCategoryRepository = blindDateCategoryRepository;
            _loggerManager = loggerManager;
        }
        public async Task<ErrorOr<AddBlindDateCategoryResult>> Handle(AddBlindDateCategoryCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting blind date category creation process for category name: {request.CategoryName}");
            if(string.IsNullOrEmpty(request.CategoryName) )
            {
                _loggerManager.LogError($"Category name is required for blind date category creation: Aborting creation.");
                return Errors.Dating.NoBlindDateCategoryName;
            }
            var existingCategory = await _blindDateCategoryRepository.GetByCategoryName(request.CategoryName);
            if (existingCategory != null)
            {
                _loggerManager.LogWarn($"Duplicate blind date category detected: {request.CategoryName}. Aborting creation.");
                return Errors.Dating.DuplicateBlindDateCategory;
            }

            //Create new blind date category object
            var category = new Domain.Entities.DatingEnvironment.BlindDates.BlindDateCategory
            {
                CategoryName = request.CategoryName,
                Description = request.Description,
            };
            await _blindDateCategoryRepository.AddAsync(category);
            _loggerManager.LogInfo($"Successfully added new blind date category: {category.CategoryName} with ID: {category.Id}");

            return new AddBlindDateCategoryResult(category.Id, category.CategoryName);
        }
    }
}
