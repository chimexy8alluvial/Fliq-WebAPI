using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Profile.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fliq.Application.Authentication.Queries.SetupData
{
    public record GetProfileSetupDataQuery() : IRequest<ErrorOr<ProfileDataTablesResponse>>;

    public class GetProfileSetupDataQueryHandler : IRequestHandler<GetProfileSetupDataQuery, ErrorOr<ProfileDataTablesResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetProfileSetupDataQueryHandler> _logger;
        public GetProfileSetupDataQueryHandler(IUserRepository userRepository, ILogger<GetProfileSetupDataQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<ProfileDataTablesResponse>> Handle(GetProfileSetupDataQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching profile setup data");
                var result = await _userRepository.GetAllProfileSetupData(cancellationToken);

                if (result.IsError)
                {
                    _logger.LogWarning("Failed to fetch profile setup data: {Errors}", result.Errors);
                }
                else
                {
                    _logger.LogInformation("Successfully fetched profile setup data");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching profile setup data");
                return Error.Unexpected(description: "An unexpected error occurred");
            }
        }

    }
}
