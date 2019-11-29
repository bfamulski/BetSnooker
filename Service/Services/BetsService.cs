using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Models;
using BetSnooker.Repositories;

namespace BetSnooker.Services
{
    public interface IBetsService
    {
        Task<IEnumerable<RoundBets>> GetAllBets(int roundId);
        Task<RoundBets> GetBets(string userId, int roundId);
        Task SubmitBets(string userId, RoundBets bets);
    }

    public class BetsService : IBetsService
    {
        private readonly IBetsRepository _betsRepository;
        private readonly ISnookerFeedService _snookerFeedService;
        private readonly IAdminService _adminService;

        public BetsService(IBetsRepository betsRepository, ISnookerFeedService snookerFeedService, IAdminService adminService)
        {
            _betsRepository = betsRepository;
            _snookerFeedService = snookerFeedService;
            _adminService = adminService;
        }

        public async Task<IEnumerable<RoundBets>> GetAllBets(int roundId)
        {
            // TODO: allow when round starts
            return await Task.Run(() => _betsRepository.GetAllBets(roundId));
        }

        public async Task<RoundBets> GetBets(string userId, int roundId)
        {
            var result = await Task.Run(() => _betsRepository.GetBets(userId, roundId));
            if (result == null)
            {
                var eventId = await _adminService.GetCurrentEventId();
                var matches = await _snookerFeedService.GetRoundMatches(roundId);
                var players = await _snookerFeedService.GetEventPlayers();

                var roundBets = new RoundBets
                {
                    UserId = userId,
                    EventId = eventId,
                    RoundId = roundId,
                    MatchBets = new List<Bet>()
                };

                foreach (var match in matches)
                {
                    var bet = new Bet
                    {
                        MatchId = match.MatchId,
                        Player1Id = match.Player1Id,
                        Player1Name = players.Single(p => p.Id == match.Player1Id).ToString(),
                        Score1 = null,
                        Player2Id = match.Player2Id,
                        Player2Name = players.Single(p => p.Id == match.Player2Id).ToString(),
                        Score2 = null
                    };

                    roundBets.MatchBets.Add(bet);
                }

                return roundBets;
            }

            return result;
        }

        public async Task SubmitBets(string userId, RoundBets bets)
        {
            var eventId = await _adminService.GetCurrentEventId();
            bets.EventId = eventId;
            await Task.Run(() => _betsRepository.SubmitBets(userId, bets));
        }
    }
}