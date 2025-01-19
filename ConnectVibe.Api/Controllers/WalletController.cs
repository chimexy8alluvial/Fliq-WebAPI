using Fliq.Application.Payments.Commands.CreateWallet;
using Fliq.Application.Payments.Queries.GetWallet;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WalletController> _logger;
        private readonly IMapper _mapper;

        public WalletController(IMediator mediator, ILogger<WalletController> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet()
        {
            var userId = GetAuthUserId();
            var command = new CreateWalletCommand(userId);
            var result = await _mediator.Send(command);
            return result.Match(
                wallet => Ok(wallet),
                errors => Problem(errors.First().Description)
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userId = GetAuthUserId();
            var query = new GetWalletQuery(userId);
            var result = await _mediator.Send(query);
            return result.Match(
                balance => Ok(new { Balance = balance }),
                errors => Problem(errors.First().Description)
            );
        }
    }
}