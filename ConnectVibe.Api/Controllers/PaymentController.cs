using Fliq.Application.Common.Interfaces.Services;
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
        private readonly ILoggerManager _logger;

        public PaymentController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleRevenueCatWebhook([FromBody] RevenueCatWebhookPayload payload)
        {
            _logger.LogInfo($"RevenueCatWebhook Request Received: {payload}");
            var command = _mapper.Map<RevenueCatWebhookCommand>(payload);
            var result = await _mediator.Send(command);
            _logger.LogInfo($"RevenueCatWebhook Command Executed. Result: {result}");

            return result.Match(
               result => Ok(_mapper.Map<PaymentResponse>(result)),
               errors => Problem(errors)
               );
        }
    }
}