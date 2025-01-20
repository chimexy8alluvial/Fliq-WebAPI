using Fliq.Application.Common.Hubs;
using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Application.Games.Commands.CreateQuestion;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Application.Games.Common;
using Fliq.Application.Games.Queries.GetGame;
using Fliq.Application.Games.Queries.GetGameHistory;
using Fliq.Application.Games.Queries.GetGames;
using Fliq.Application.Games.Queries.GetQuestions;
using Fliq.Application.Games.Queries.GetSession;
using Fliq.Contracts.Games;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fliq.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GamesController> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<GameHub> _hubContext;

        public GamesController(IMediator mediator, ILogger<GamesController> logger, IMapper mapper, IHubContext<GameHub> hubContext)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hubContext = hubContext;
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

        [HttpGet("questions/paginated")]
        public async Task<IActionResult> GetGameQuestions([FromQuery] GetQuestionsRequest request)
        {
            _logger.LogInformation($"Get Game Questions Request Received for Game with ID: {request.GameId}");
            var query = _mapper.Map<GetQuestionsQuery>(request);
            var result = await _mediator.Send(query);
            _logger.LogInformation($"Get Game Questions Query Executed. Result: {result}");

            return result.Match(
                session => Ok(_mapper.Map<List<GetQuestionResult>>(session)),
                errors => Problem(errors)
            );
        }

        [HttpPost("submit-scores")]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitAnswer([FromForm] SubmitAnswerDto request)
        {
            _logger.LogInformation($"Submit Answer Request Received: {request}");
            var command = _mapper.Map<SubmitAnswerCommand>(request);
            var result = await _mediator.Send(command);
            _logger.LogInformation($"Submit Answer Command Executed. Result: {result}");

            return result.Match(
                success => Ok("Score submitted successfully."),
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

        [HttpGet("GetGameHistory")]
        public async Task<IActionResult> GetGameHistory(int player1Id, int player2Id)
        {
            var query = new GetGameHistoryQuery(player1Id, player2Id);

            var result = await _mediator.Send(query);

            return result.Match(
              session => Ok(_mapper.Map<List<GameSessionResponse>>(session)),
              errors => Problem(errors)
          );
        }

        [HttpPost("join-session")]
        [AllowAnonymous]
        public async Task<IActionResult> JoinSession([FromForm] string sessionId)
        {
            var connectionId = HttpContext.Connection.Id;
            await _hubContext.Clients.Group(sessionId).SendAsync("PlayerJoined", connectionId);
            return Ok();
        }

        [HttpPost("leave-session")]
        [AllowAnonymous]
        public async Task<IActionResult> LeaveSession([FromForm] string sessionId)
        {
            var connectionId = HttpContext.Connection.Id;
            await _hubContext.Clients.Group(sessionId).SendAsync("PlayerLeft", connectionId);
            return Ok();
        }
    }
}