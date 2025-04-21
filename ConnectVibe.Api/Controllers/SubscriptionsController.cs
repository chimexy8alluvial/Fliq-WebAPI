using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Subscriptions.Commands;
using Fliq.Contracts.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly ILoggerManager _logger;

        public SubscriptionsController(ISender mediator, ILoggerManager logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("SubscriptionPlan")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanCommand request)
        {
            _logger.LogInfo($"Add Subscription Plan request received: {request}");
         
            var result = await _mediator.Send(request);
            _logger.LogInfo($"Add Subscription Plan Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new SubscriptionPlanResponse()),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("SubscriptionPlanPrice")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubscriptionPlanPrice([FromBody] AddSubscriptionPlanPriceCommand request)
        {
            _logger.LogInfo($"Add Subscription Plan Price request received: {request}");

            var result = await _mediator.Send(request);
            _logger.LogInfo($"Add Subscription Plan Price Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new SubscriptionPlanResponse()),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

    }

}
