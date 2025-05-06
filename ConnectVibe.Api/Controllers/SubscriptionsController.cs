using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Subscriptions.Commands;
using Fliq.Application.Subscriptions.Queries;
using Fliq.Contracts.Subscriptions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SubscriptionsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public SubscriptionsController(ISender mediator, ILoggerManager logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("SubscriptionPlan")]
        public async Task<IActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequestDto request)
        {
            _logger.LogInfo($"Add Subscription Plan request received: {request}");
            var command = _mapper.Map<CreateSubscriptionPlanCommand>(request);
            var result = await _mediator.Send(command);
            _logger.LogInfo($"Add Subscription Plan Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new SubscriptionPlanResponse()),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("SubscriptionPlanPrice")]
        public async Task<IActionResult> CreateSubscriptionPlanPrice([FromBody] AddSubscriptionPlanPriceRequestDto request)
        {
            _logger.LogInfo($"Add Subscription Plan Price request received: {request}");
            var command = _mapper.Map<AddSubscriptionPlanPriceCommand>(request);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Add Subscription Plan Price Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new SubscriptionPlanResponse()),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("SubscriptionPlans")]
        [Produces(typeof(SubscriptionPlanDto))]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            _logger.LogInfo($" Subscription Plans Query received");
            var userId = GetAuthUserId();

            var query = new GetSubscriptionPlansQuery(userId);

            var result = await _mediator.Send(query);
            _logger.LogInfo($"Prompt Categories Query Executed. Result: {result}");

            return result.Match(
                result => Ok(result),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }
    }

}
