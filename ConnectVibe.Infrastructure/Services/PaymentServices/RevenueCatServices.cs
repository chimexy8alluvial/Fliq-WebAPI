using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;

namespace Fliq.Infrastructure.Services.PaymentServices
{
    public class RevenueCatServices : IRevenueCatServices
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILoggerManager _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public RevenueCatServices(IPaymentRepository paymentRepository, ILoggerManager logger, IHttpClientFactory httpClientFactory)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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
                    Method = PaymentMethod.Unknown,
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

        public async Task<bool> ProcessCancellationOrExpirationAsync(RevenueCatWebhookPayload payload)
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

        public async Task<ErrorOr<bool>> RefundTransactionAsync(string transactionId)
        {
            try
            {
                await Task.CompletedTask;
                if (string.IsNullOrEmpty(transactionId))
                {
                    _logger.LogError("Refund failed: Transaction ID is null or empty.");
                    return Errors.Payment.InvalidPaymentTransaction;
                }

                var payment = _paymentRepository.GetPaymentByTransactionId(transactionId);
                if (payment == null)
                {
                    _logger.LogError($"Refund failed: No payment found for transaction ID {transactionId}.");
                    return Errors.Payment.PaymentNotFound;
                }

                if (payment.Status == PaymentStatus.Refunded)
                {
                    _logger.LogError($"Refund failed: Payment for transaction ID {transactionId} is already refunded.");
                    return Error.Conflict(description: "Payment is already refunded.");
                }

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer your_revenuecat_api_key");
                var response = await httpClient.PostAsync(
                    $"https://api.revenuecat.com/v1/subscribers/{payment.UserId}/transactions/{transactionId}/refund",
                    null);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"RevenueCat API refund failed for transaction ID {transactionId}.");
                    return Errors.Payment.FailedToProcess;
                }

                payment.Status = PaymentStatus.Refunded;
                payment.DateModified = DateTime.UtcNow; 
                _paymentRepository.Update(payment);

                _logger.LogInfo($"Successfully refunded transaction {transactionId}.");
                return true; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to refund transaction {transactionId}: {ex.Message}");
                return Errors.Payment.FailedToProcess;
            }
        }


    }
}