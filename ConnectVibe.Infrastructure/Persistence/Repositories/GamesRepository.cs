using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Games.Common;
using Fliq.Domain.Entities.Games;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public GamesRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public Game? GetGameById(int id)
        {
            var profile = _dbContext.Games.SingleOrDefault(p => p.Id == id);
            return profile;
        }

        public void AddGame(Game game)
        {
            if (game.Id > 0)
            {
                _dbContext.Update(game);
            }
            else
            {
                _dbContext.Add(game);
            }
            _dbContext.SaveChanges();
        }

        public void UpdateGame(Game game)
        {
            _dbContext.Update(game);

            _dbContext.SaveChanges();
        }

        public IEnumerable<Game> GetAllGames()
        {
            var games = _dbContext.Games;
            return games;
        }

        public async Task<int> CountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountGames", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> FlaggedCountAsync()
        {

            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountFlaggedEvents", commandType: CommandType.StoredProcedure);
                return count;
            }
        }
        public void CreateGameSession(GameSession gameSession)
        {
            if (gameSession.Id > 0)
            {
                _dbContext.Update(gameSession);
            }
            else
            {
                _dbContext.Add(gameSession);
            }
            _dbContext.SaveChanges();
        }

        public void UpdateGameSession(GameSession gameSession)
        {
            _dbContext.Update(gameSession);
            _dbContext.SaveChanges();
        }

        public GameSession? GetGameSessionById(int id)
        {
            var session = _dbContext.GameSessions
                .SingleOrDefault(p => p.Id == id);
            return session;
        }

        public async Task<int> GetStakeCountByUserId(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_GetSingleUserTotalStakeCount";
                var parameter = new { UserId = userId };
                
                var count = await connection.QueryFirstOrDefaultAsync<int>(sql, commandType: CommandType.StoredProcedure); // Using IsActive flag
                return count;
            }
        }

        public List<GetGameHistoryResult> GetGameHistoryByTwoPlayers(int player1Id, int player2Id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                const string storedProcedure = "GetGameHistoryByTwoPlayers";

                var result = connection.Query<GetGameHistoryResult>(
                    storedProcedure,
                    new { Player1Id = player1Id, Player2Id = player2Id },
                    commandType: CommandType.StoredProcedure
                ).ToList();
                return result;
            }
        }

        public void AddGameRequest(GameRequest gameRequest)
        {
            if (gameRequest.Id > 0)
            {
                _dbContext.Update(gameRequest);
            }
            else
            {
                _dbContext.Add(gameRequest);
            }
            _dbContext.SaveChanges();
        }

        public void UpdateGameRequest(GameRequest gameRequest)
        {
            _dbContext.Update(gameRequest);
            _dbContext.SaveChanges();
        }

        public GameRequest? GetGameRequestById(int id)
        {
            var request = _dbContext.GameRequests.SingleOrDefault(p => p.Id == id);
            return request;
        }

        public void AddQuestion(GameQuestion question)
        {
            _dbContext.GameQuestions.Add(question);
            _dbContext.SaveChanges();
        }

        public GameQuestion? GetQuestionById(int id)
        {
            return _dbContext.GameQuestions.SingleOrDefault(q => q.Id == id);
        }

        public List<GameQuestion> GetQuestionsByGameId(int gameId, int pageNumber, int pageSize)
        {
            // Call stored procedure with pagination parameters
            return _dbContext.GameQuestions
                .FromSqlRaw(
                    "EXEC GetQuestionsByGameIdPaginated @GameId = {0}, @PageNumber = {1}, @PageSize = {2}",
                    gameId, pageNumber, pageSize)
                .ToList();
        }
    }
}