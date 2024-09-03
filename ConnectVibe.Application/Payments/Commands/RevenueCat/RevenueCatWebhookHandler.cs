using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Payments.Common;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Payments.Commands.RevenueCat
{
    public class RevenueCatWebhookHandler : IRequestHandler<RevenueCatWebhookCommand, ErrorOr<RevenueCatWebhookResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IRevenueCatServices _revenueCatServices;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public RevenueCatWebhookHandler(IUserRepository userRepository, ISubscriptionService subscriptionService, IRevenueCatServices revenueCatServices, IMapper mapper, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _subscriptionService = subscriptionService;
            _revenueCatServices = revenueCatServices;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ErrorOr<RevenueCatWebhookResult>> Handle(RevenueCatWebhookCommand command, CancellationToken cancellationToken)
        {
            // Validate user existence
            var userId = int.Parse(command.Event.AppUserId);
            var user = _userRepository.GetUserById(userId);

            if (user == null)
            {
                return Errors.User.UserNotFound;
            }

            // Map the command to the payload object
            RevenueCatWebhookPayload payload = _mapper.Map<RevenueCatWebhookPayload>(command);

            bool operationResult = false;

            // Handle different event types
            switch (command.Event.Type)
            {
                case "INITIAL_PURCHASE":
                    operationResult = await _revenueCatServices.ProcessInitialPurchaseAsync(payload);
                    if (operationResult)
                    {
                        await _subscriptionService.ActivateSubscriptionAsync(payload);
                    }
                    break;

                case "RENEWAL":
                    operationResult = await _revenueCatServices.ProcessRenewalAsync(payload);
                    if (operationResult)
                    {
                        await _subscriptionService.ExtendSubscriptionAsync(payload);
                    }
                    break;

                // Handle other events like CANCELLATION, EXPIRATION, etc.
                default:
                    return Errors.Payment.InvalidPayload;
            }

            // Return appropriate response based on the operation result
            if (operationResult)
            {
                return new RevenueCatWebhookResult
                {
                    Success = true,
                    Message = "Operation completed successfully."
                };
            }
            else
            {
                return Errors.Payment.FailedToProcess;
            }
        }
    }
}