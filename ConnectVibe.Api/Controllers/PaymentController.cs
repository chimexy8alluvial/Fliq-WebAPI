using ConnectVibe.Api.Controllers;
using Fliq.Application.Payments.Commands.RevenueCat;
using Fliq.Application.Payments.Common;
using Fliq.Contracts.Payments;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PaymentController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public PaymentController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> HandleRevenueCatWebhook([FromBody] RevenueCatWebhookPayload payload)
        {
            var command = _mapper.Map<RevenueCatWebhookCommand>(payload);
            var result = await _mediator.Send(command);
            return result.Match(
               result => Ok(_mapper.Map<PaymentResponse>(result)),
               errors => Problem(errors)
               );
        }
    }
}