
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Prompts.Common;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Prompts.Queries
{
    public record GetPromptCategoriesQuery(int UserId) : IRequest<ErrorOr<List<GetPromptCategoriesResult>>>;

    public class GetProfileQueryHandler : IRequestHandler<GetPromptCategoriesQuery, ErrorOr<List<GetPromptCategoriesResult>>>
    {
        private readonly IPromptCategoryRepository _promptCategoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public GetProfileQueryHandler(IPromptCategoryRepository promptCategoryRepository, IUserRepository userRepository, ILoggerManager loggerManager, IMapper mapper)
        {
            _promptCategoryRepository = promptCategoryRepository;
            _userRepository = userRepository;
            _logger = loggerManager;
            _mapper = mapper;
        }

        public async Task<ErrorOr<List<GetPromptCategoriesResult>>> Handle(GetPromptCategoriesQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(request.UserId);
            if(user == null)
            {
                _logger.LogError($"User with id {request.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            var categories = _promptCategoryRepository.GetAllPromptCategories();

            return _mapper.Map<List<GetPromptCategoriesResult>>(categories);
        }
    }
}
