using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MediatR;

namespace Fliq.Application.Payments.Commands.CreateWallet
{
    public record CreateWalletCommand(int UserId) : IRequest<ErrorOr<Wallet>>;

    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, ErrorOr<Wallet>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public CreateWalletCommandHandler(IWalletRepository walletRepository, IUserRepository userRepository, ILoggerManager logger)
        {
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Wallet>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            _logger.LogInfo($"Creating wallet for user {request.UserId}");

            var user = _userRepository.GetUserById(request.UserId);
            if (user is null)
            {
                _logger.LogError("User not found");
                return Errors.User.UserNotFound;
            }
            var existingWallet = _walletRepository.GetWalletByUserId(request.UserId);
            if (existingWallet != null)
            {
                _logger.LogError("Wallet already exists");
                return Errors.Wallet.AlreadyExists;
            }

            var wallet = new Wallet
            {
                UserId = request.UserId,
            };

            _walletRepository.Add(wallet);
            return wallet;
        }
    }
}