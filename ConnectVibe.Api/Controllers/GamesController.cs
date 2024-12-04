using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Application.Games.Commands.CreateQuestion;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Application.Games.Queries.GetGame;
using Fliq.Application.Games.Queries.GetGames;
using Fliq.Application.Games.Queries.GetSession;
using Fliq.Contracts.Games;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GamesController> _logger;
        private readonly IMapper _mapper;

        public GamesController(IMediator mediator, ILogger<GamesController> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromForm] CreateGameDto request)
        {
            _logger.LogInformation($"Create Game Request Received: {request}");
            var command = _mapper.Map<CreateGameCommand>(request);
            var gamesResult = await _mediator.Send(command);
            _logger.LogInformation($"Create Game Command Executed. Result: {gamesResult}");

            return gamesResult.Match(
                gamesResult => Ok(_mapper.Map<GetGameResponse>(gamesResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("send-game-request")]
        public async Task<IActionResult> SendGameRequest([FromForm] SendGameRequestDto request)
        {
            _logger.LogInformation($"Send Game Request Received: {request}");
            var command = _mapper.Map<SendGameRequestCommand>(request);
            var userId = GetAuthUserId();
            command = command with { RequesterId = userId };
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Send Game Request Command Executed. Result: {result}");

            return result.Match(
                success => Ok("Request sent successfully."),
                errors => Problem(errors)
            );
        }

        [HttpPost("accept-game-request")]
        public async Task<IActionResult> AcceptGameRequest([FromForm] AcceptGameRequestDto request)
        {
            _logger.LogInformation($"Accept Game Request Received: {request}");
            var command = _mapper.Map<AcceptGameRequestCommand>(request);
            var userId = GetAuthUserId();
            command = command with { UserId = userId };
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Accept Game Request Command Executed. Result: {result}");

            return result.Match(
                success => Ok("Game request accepted."),
                errors => Problem(errors)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            _logger.LogInformation($"Get Game Request Received for Game ID: {id}");
            var query = new GetGameByIdQuery(id);
            var result = await _mediator.Send(query);
            _logger.LogInformation($"Get Game Query Executed. Result: {result}");

            return result.Match(
                gameResult => Ok(_mapper.Map<GetGameResponse>(gameResult)),
                errors => Problem(errors)
            );
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllGames()
        {
            _logger.LogInformation("Get All Games Request Received");
            var query = new GetAllGamesQuery();
            var result = await _mediator.Send(query);
            _logger.LogInformation($"Get All Games Query Executed. Result: {result.Count} games found.");

            return Ok(result.Select(_mapper.Map<GetGameResponse>).ToList());
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetGameSession(int sessionId)
        {
            _logger.LogInformation($"Get Game Session Request Received for Session ID: {sessionId}");
            var query = new GetGameSessionQuery(sessionId);
            var result = await _mediator.Send(query);
            _logger.LogInformation($"Get Game Session Query Executed. Result: {result}");

            return result.Match(
                session => Ok(_mapper.Map<GetGameSessionResponse>(session)),
                errors => Problem(errors)
            );
        }

        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer([FromForm] SubmitAnswerDto request)
        {
            _logger.LogInformation($"Submit Answer Request Received: {request}");
            var command = _mapper.Map<SubmitAnswerCommand>(request);
            var userId = GetAuthUserId();
            command = command with { PlayerId = userId };
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Submit Answer Command Executed. Result: {result}");

            return result.Match(
                success => Ok("Answer submitted successfully."),
                errors => Problem(errors)
            );
        }

        [HttpPost("questions/create")]
        public async Task<IActionResult> CreateQuestion([FromForm] CreateQuestionDto request)
        {
            _logger.LogInformation($"Create Question Request Received: {request}");
            var command = _mapper.Map<CreateQuestionCommand>(request);
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Create Question Command Executed. Result: {result}");

            return result.Match(
                questionResult => Ok(_mapper.Map<GetQuestionResponse>(questionResult)),
                errors => Problem(detail: string.Join(", ", errors.Select(e => e.Description)), statusCode: 400)
            );
        }
    }
}