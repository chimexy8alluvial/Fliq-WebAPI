using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MediatR;

namespace Fliq.Application.Payments.Queries.GetWallet
{
    public record GetWalletQuery(int UserId) : IRequest<ErrorOr<Wallet>>;

    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, ErrorOr<Wallet>>
    {
        private readonly IWalletRepository _walletRepository;

        public GetWalletQueryHandler(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<ErrorOr<Wallet>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            var wallet = _walletRepository.GetWalletByUserId(request.UserId);
            if (wallet == null)
                return Errors.Wallet.NotFound;

            return wallet;
        }
    }
}