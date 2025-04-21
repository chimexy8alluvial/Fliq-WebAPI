using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Payments.Commands.RevenueCat
{
    public class RevenueCatWebhookCommand : IRequest<ErrorOr<RevenueCatWebhookResult>>
    {
        public EventInfo Event { get; set; } = default!;
        public string ApiVersion { get; set; } = default!;

        public class EventInfo
        {
            public long EventTimestampMs { get; set; } = default!;
            public string ProductId { get; set; } = default!;
            public string PeriodType { get; set; } = default!;
            public long PurchasedAtMs { get; set; } = default!;
            public long ExpirationAtMs { get; set; } = default!;
            public string Environment { get; set; } = default!;
            public string EntitlementId { get; set; } = default!;
            public List<string> EntitlementIds { get; set; } = default!;
            public string PresentedOfferingId { get; set; } = default!;
            public string TransactionId { get; set; } = default!;
            public string OriginalTransactionId { get; set; } = default!;
            public bool IsFamilyShare { get; set; } = default!;
            public string CountryCode { get; set; } = default!;
            public string AppUserId { get; set; } = default!;
            public List<string> Aliases { get; set; } = default!;
            public string OriginalAppUserId { get; set; } = default!;
            public string Currency { get; set; } = default!;
            public decimal Price { get; set; } = default!;
            public decimal PriceInPurchasedCurrency { get; set; } = default!;
            public Dictionary<string, SubscriberAttribute> SubscriberAttributes { get; set; } = default!;
            public string Store { get; set; } = default!;
            public double TakehomePercentage { get; set; } = default!;
            public string OfferCode { get; set; } = default!;
            public string Type { get; set; } = default!;
            public string Id { get; set; } = default!;
            public string AppId { get; set; } = default!;
        }

        public class SubscriberAttribute
        {
            public long UpdatedAtMs { get; set; } = default!;
            public string Value { get; set; } = default!;
        }
    }

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
                _logger.LogError($"User with ID {userId} not found.");
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
                case "CANCELLATION":
                    await _subscriptionService.ProcessCancellationAsync(payload);
                    break;
                case "EXPIRATION":
                    await _subscriptionService.DeactivateExpiredUserSubscriptionAsync(payload);
                    break;
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
                _logger.LogError("Failed to process payment.");
                return Errors.Payment.FailedToProcess;
            }
        }
    }
}