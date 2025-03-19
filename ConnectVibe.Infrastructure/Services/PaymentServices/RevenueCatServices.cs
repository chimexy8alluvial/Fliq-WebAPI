using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities;
using Environment = Fliq.Domain.Entities.Environment;

namespace Fliq.Infrastructure.Services.PaymentServices
{
    public class RevenueCatServices : IRevenueCatServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILoggerManager _logger;

        public RevenueCatServices(IPaymentRepository paymentRepository, ILoggerManager logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task<bool> ProcessInitialPurchaseAsync(RevenueCatWebhookPayload payload)
        {
            try
            {
                await Task.CompletedTask;
                var payment = new Payment
                {
                    UserId = int.Parse(payload.Event.AppUserId),
                    Provider = PaymentProvider.RevenueCat,
                    TransactionId = payload.Event.TransactionId,
                    ProductId = payload.Event.ProductId,
                    Amount = payload.Event.Price,
                    Currency = payload.Event.Currency,
                    PaymentDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.PurchasedAtMs).UtcDateTime,
                    Status = PaymentStatus.Success,
                    Method = PaymentMethod.Unkmown,
                    Environment = payload.Event.Environment == "PRODUCTION" ? Environment.Production : Environment.Sandbox
                };
                _paymentRepository.Add(payment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> ProcessRenewalAsync(RevenueCatWebhookPayload payload)
        {
            try
            {
                await Task.CompletedTask;
                var payment = new Payment
                {
                    UserId = int.Parse(payload.Event.AppUserId),
                    Provider = PaymentProvider.RevenueCat,
                    TransactionId = payload.Event.TransactionId,
                    ProductId = payload.Event.ProductId,
                    Amount = payload.Event.Price,
                    Currency = payload.Event.Currency,
                    PaymentDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.PurchasedAtMs).UtcDateTime,
                    Status = PaymentStatus.Success,
                    Method = PaymentMethod.ApplePay,
                    Environment = payload.Event.Environment == "PRODUCTION" ? Environment.Production : Environment.Sandbox
                };
                _paymentRepository.Add(payment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}