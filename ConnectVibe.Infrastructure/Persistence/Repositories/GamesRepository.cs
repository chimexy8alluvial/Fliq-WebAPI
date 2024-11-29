﻿using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Games;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly FliqDbContext _dbContext;

        public GamesRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var session = _dbContext.GameSessions.SingleOrDefault(p => p.Id == id);
            return session;
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
    }
}