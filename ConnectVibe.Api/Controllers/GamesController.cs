using Fliq.Application.Common.Hubs;
using Fliq.Application.Common.Interfaces.UserFeatureActivities;
using Fliq.Application.Games.Commands.AcceptGameRequest;
using Fliq.Application.Games.Commands.AcceptStake;
using Fliq.Application.Games.Commands.CreateGame;
using Fliq.Application.Games.Commands.CreateQuestion;
using Fliq.Application.Games.Commands.CreateStake;
using Fliq.Application.Games.Commands.RejectStake;
using Fliq.Application.Games.Commands.SendGameRequest;
using Fliq.Application.Games.Commands.SubmitAnswer;
using Fliq.Application.Games.Common;
using Fliq.Application.Games.Queries.GetActiveGamesCountQuery;
using Fliq.Application.Games.Queries.GetGame;
using Fliq.Application.Games.Queries.GetGameHistory;
using Fliq.Application.Games.Queries.GetGames;
using Fliq.Application.Games.Queries.GetNumberOfGamersCountQuery;
using Fliq.Application.Games.Queries.GetQuestions;
using Fliq.Application.Games.Queries.GetSession;
using Fliq.Application.Games.Queries.GetTotalGamesPlayed;
using Fliq.Application.Games.Queries.StakeCount;
using Fliq.Contracts.DashBoard;
using Fliq.Contracts.Games;
using Mapster;
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
        private readonly IUserFeatureActivityService _userFeatureActivityService;

        public GamesController(IMediator mediator, ILogger<GamesController> logger, IMapper mapper, IHubContext<GameHub> hubContext, IUserFeatureActivityService userFeatureActivityService)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hubContext = hubContext;
            _userFeatureActivityService = userFeatureActivityService;
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
                session => Ok(_mapper.Map<GameSessionResponse>(session)),
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

        [HttpPost("create-stake")]
        public async Task<IActionResult> CreateStake([FromBody] CreateStakeRequestDto requestDto)
        {
            var userId = GetAuthUserId();

            // Track Feature Activity
            await _userFeatureActivityService.TrackUserFeatureActivity(userId, "Create-GameStake");

            var command = requestDto.Adapt<CreateStakeCommand>();
            command = command with { RequesterId = userId };
            var result = await _mediator.Send(command);

            return result.Match(
                stake => Ok(stake.Adapt<StakeResponseDto>()),
                errors => Problem(errors)
            );
        }

        [HttpPost("accept-stake")]
        public async Task<IActionResult> AcceptStake([FromBody] AcceptStakeRequestDto requestDto)
        {
            var userId = GetAuthUserId();

            // Track Feature Activity
            await _userFeatureActivityService.TrackUserFeatureActivity(userId, "Accept-GameStake");

            var command = requestDto.Adapt<AcceptStakeCommand>();
            command = command with { UserId = userId };
            var result = await _mediator.Send(command);

            return result.Match(
                stake => Ok(stake.Adapt<StakeResponseDto>()),
                errors => Problem(errors.First().Description)
            );
        }

        [HttpPost("reject-stake")]
        public async Task<IActionResult> RejectStake([FromBody] AcceptStakeRequestDto requestDto)
        {
            var userId = GetAuthUserId();

            // Track Feature Activity
            await _userFeatureActivityService.TrackUserFeatureActivity(userId, "Reject-GameStake");

            var command = requestDto.Adapt<RejectStakeCommand>();
            command = command with { UserId = userId };
            var result = await _mediator.Send(command);

            return result.Match(
                stake => Ok(stake.Adapt<StakeResponseDto>()),
                errors => Problem(errors.First().Description)
            );
        }
        /*---Admin fxns ----------*/
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("users-count")]
        public async Task<IActionResult> GetUsersCount([FromQuery] int userId)
        {
            _logger.LogInformation("Received request for inactive users count.");

            var query = new GetUserStakeCountQuery(userId);

            var result = await _mediator.Send(query);

            return result.Match(
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("active-games-count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetActiveGamesCount()
        {
            _logger.LogInformation("Recieved request for active games count");

            var query = new GetActiveGamesCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
                matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("number-of-gamers-count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetNumberOfGamersCount()
        {
            _logger.LogInformation("Recieved request for number of gamers count");

            var query = new GetNumberOfGamersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
                matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("total-games-played-count")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetTotalGamesPlayedCount()
        {
            _logger.LogInformation("Recieved request for total games played count");

            var query = new GetTotalGamesPlayedCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
                matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }



    }
}