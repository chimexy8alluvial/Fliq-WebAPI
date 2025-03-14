using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Common.BlindDateCategory;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Queries.BlindDateCategory
{
    public record GetBlindDateCategoryByIdQuery(int Id) : IRequest<ErrorOr<GetBlindDateCategoryResult>>;

    public class GetBlindDateCategoryByIdQueryHandler
        : IRequestHandler<GetBlindDateCategoryByIdQuery, ErrorOr<GetBlindDateCategoryResult>>
    {
        private readonly IBlindDateCategoryRepository _blindDateCategoryRepository;
        private readonly ILoggerManager _loggerManager;

        public GetBlindDateCategoryByIdQueryHandler(IBlindDateCategoryRepository blindDateCategoryRepository, ILoggerManager loggerManager)
        {
            _blindDateCategoryRepository = blindDateCategoryRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<GetBlindDateCategoryResult>> Handle(GetBlindDateCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Fetching blind date category ID: {request.Id}");

            var category = await _blindDateCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                _loggerManager.LogWarn($"Blind date category ID: {request.Id} not found.");
                return Errors.Dating.BlindDateCategoryNotFound;
            }
            _loggerManager.LogInfo($"Successfully retrieved blind date category: {category.CategoryName} (ID: {category.Id})");

            return new GetBlindDateCategoryResult(category.Id, category.CategoryName, category.Description);
        }
    }
}
