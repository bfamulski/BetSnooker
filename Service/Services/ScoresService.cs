using System.Collections.Generic;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Repositories;

namespace BetSnooker.Services
{
    public interface IScoresService
    {
        Task<IEnumerable<Score>> GetAllScores();
        Task<Score> GetScore(string userId, int eventId, int? roundId, int? matchId);
        Task SaveScore(Score score);
    }

    public class ScoresService : IScoresService
    {
        private readonly IScoresRepository _scoresRepository;
        private readonly IBetsService _betsService;

        public ScoresService(IScoresRepository scoresRepository, IBetsService betsService)
        {
            _scoresRepository = scoresRepository;
            _betsService = betsService;
        }

        public async Task<IEnumerable<Score>> GetAllScores()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Score> GetScore(string userId, int eventId, int? roundId, int? matchId)
        {
            return await Task.Run(() => _scoresRepository.GetScore(userId, eventId, roundId, matchId));
        }

        public async Task SaveScore(Score score)
        {
            await Task.Run(() => _scoresRepository.SaveScore(score));
        }
    }
}