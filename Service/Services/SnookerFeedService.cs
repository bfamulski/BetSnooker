using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.HttpHelper;
using BetSnooker.Models;
using BetSnooker.Models.API;

namespace BetSnooker.Services
{
    public interface ISnookerFeedService
    {
        // events
        Task<IEnumerable<Event>> GetEvents(int season);
        Task<Event> GetEvent(int eventId);

        // rounds
        Task<IEnumerable<RoundInfo>> GetAllRoundsInfo();
        Task<RoundInfo> GetRoundInfo(int roundId);

        // matches
        Task<IEnumerable<Match>> GetEventMatches();
        Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId);
        Task<Match> GetMatch(int roundId, int matchNumber);

        // players
        Task<IEnumerable<Player>> GetEventPlayers();
        Task<Player> GetPlayer(int playerId);
    }

    public class SnookerFeedService : ISnookerFeedService
    {
        private const string SnookerApiUrl = "http://api.snooker.org/";

        private readonly IAsyncRestClient _restClient;
        private readonly IAdminService _adminService;

        // TODO: implement better caching
        private readonly IDictionary<int, IEnumerable<Player>> _cachedPlayers = new Dictionary<int, IEnumerable<Player>>();
        private readonly IDictionary<int, IEnumerable<RoundInfo>> _cachedRounds = new Dictionary<int, IEnumerable<RoundInfo>>();

        public SnookerFeedService(IAsyncRestClient restClient, IAdminService adminService)
        {
            _restClient = restClient;
            _restClient.BaseAddress = new Uri(SnookerApiUrl);

            _adminService = adminService;
        }

        public async Task<IEnumerable<Event>> GetEvents(int season)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "5")
                .Add("s", season.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<Event>>(request);

            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Data;
        }

        public async Task<Event> GetEvent(int eventId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams().Add("e", eventId.ToString()).BuildQuery();
            var response = await _restClient.Send<IEnumerable<Event>>(request);

            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Data.FirstOrDefault();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams().Add("p", playerId.ToString()).BuildQuery();
            var response = await _restClient.Send<IEnumerable<Player>>(request);

            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<Player>> GetEventPlayers()
        {
            var eventId = await _adminService.GetCurrentEventId();

            if (_cachedPlayers.ContainsKey(eventId) && _cachedPlayers[eventId].Any())
            {
                return _cachedPlayers[eventId];
            }

            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "9")
                .Add("e", eventId.ToString())
                .BuildQuery();
            
            var response = await _restClient.Send<IEnumerable<Player>>(request);
            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            _cachedPlayers[eventId] = response.Data;
            return response.Data;
        }

        public async Task<IEnumerable<Match>> GetEventMatches()
        {
            var eventId = await _adminService.GetCurrentEventId();

            var eventMatchesRequest = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "6")
                .Add("e", eventId.ToString())
                .BuildQuery();

            var eventMatchesResponse = await _restClient.Send<IEnumerable<Match>>(eventMatchesRequest);
            if (!eventMatchesResponse.Success)
            {
                throw new Exception(eventMatchesResponse.ErrorMessage);
            }

            return eventMatchesResponse.Data;
        }

        public async Task<IEnumerable<MatchDetails>> GetRoundMatches(int roundId)
        {
            var eventId = await _adminService.GetCurrentEventId();

            var eventMatchesRequest = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "6")
                .Add("e", eventId.ToString())
                .BuildQuery();
            
            var eventMatchesResponse = await _restClient.Send<IEnumerable<Match>>(eventMatchesRequest);
            if (!eventMatchesResponse.Success)
            {
                throw new Exception(eventMatchesResponse.ErrorMessage);
            }

            var roundMatches = eventMatchesResponse.Data.Where(m => m.Round == roundId);

            var roundInfo = await GetRoundInfo(roundId);
            var players = await GetEventPlayers();

            var matchDetailsCollection = new List<MatchDetails>();
            foreach (var match in roundMatches)
            {
                var player1 = players.Single(p => p.Id == match.Player1Id);
                var player2 = players.Single(p => p.Id == match.Player2Id);
                var winner = players.SingleOrDefault(p => p.Id == match.WinnerId);

                MatchDetails matchDetails = new MatchDetails(match);
                matchDetails.Player1Name = player1.ToString();
                matchDetails.Player2Name = player2.ToString();
                matchDetails.WinnerName = winner != null ? winner.ToString() : "";
                matchDetails.RoundName = roundInfo.RoundName;
                matchDetailsCollection.Add(matchDetails);
            }

            return matchDetailsCollection;
        }

        public async Task<Match> GetMatch(int roundId, int matchNumber)
        {
            var eventId = await _adminService.GetCurrentEventId();

            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("e", eventId.ToString())
                .Add("r", roundId.ToString())
                .Add("n", matchNumber.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<Match>>(request);

            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            return response.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<RoundInfo>> GetAllRoundsInfo()
        {
            var eventId = await _adminService.GetCurrentEventId();

            if (_cachedRounds.ContainsKey(eventId) && _cachedRounds[eventId].Any())
            {
                return _cachedRounds[eventId];
            }

            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "12")
                .Add("e", eventId.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<RoundInfo>>(request);

            if (!response.Success)
            {
                throw new Exception(response.ErrorMessage);
            }

            _cachedRounds[eventId] = response.Data;
            return response.Data;
        }

        public async Task<RoundInfo> GetRoundInfo(int roundId)
        {
            var allRounds = await GetAllRoundsInfo();
            return allRounds.SingleOrDefault(r => r.Round == roundId);
        }
    }
}