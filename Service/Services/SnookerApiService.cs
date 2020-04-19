using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.HttpHelper;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class SnookerApiService :  ISnookerApiService
    {
        private readonly IAsyncRestClient _restClient;

        public SnookerApiService(IAsyncRestClient restClient, IConfigurationService configurationService)
        {
            var snookerApiUrl = configurationService.SnookerApiUrl;

            _restClient = restClient;
            _restClient.BaseAddress = new Uri(snookerApiUrl);
        }

        /// <summary>
        /// URL format: api.snooker.org/?t=5&s=[Season]
        /// Example: api.snooker.org/?t=5&s=2019 (2019/2020)
        /// Output: Event info as JSON (two-dimensional)
        /// Note: All seasons: -1
        /// </summary>
        /// <param name="season">Season</param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> GetEvents(int season)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "5")
                .Add("s", season.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<Event>>(request);

            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// URL format: api.snooker.org/?e=[Event ID]
        /// Example: api.snooker.org/?e=398 (Shanghai Masters Qualifiers 2015)
        /// Output: Event info as JSON (one-dimensional)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns></returns>
        public async Task<Event> GetEvent(int eventId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams().Add("e", eventId.ToString()).BuildQuery();
            var response = await _restClient.Send<IEnumerable<Event>>(request);

            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data.SingleOrDefault();
        }

        /// <summary>
        /// URL format: api.snooker.org/?e=[Event ID]&r=[Round ID]&n=[Match number]
        /// Example: api.snooker.org/?e=397&r=1&n=5 (Riga Open 2015, qual round 1, Mulholland v Arpat)
        /// Output: Match info as JSON (one-dimensional)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <param name="roundId">Round ID</param>
        /// <param name="matchNumber">Match number</param>
        /// <returns></returns>
        public async Task<Match> GetMatch(int eventId, int roundId, int matchNumber)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("e", eventId.ToString())
                .Add("r", roundId.ToString())
                .Add("n", matchNumber.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<Match>>(request);

            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data.SingleOrDefault();
        }

        /// <summary>
        /// URL format: api.snooker.org/?p=[Player ID]
        /// Example: api.snooker.org/?p=1 (Mark J Williams)
        /// Output: Player info as JSON (one-dimensional)
        /// </summary>
        /// <param name="playerId">Player ID</param>
        /// <returns></returns>
        public async Task<Player> GetPlayer(int playerId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams().Add("p", playerId.ToString()).BuildQuery();
            var response = await _restClient.Send<IEnumerable<Player>>(request);

            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data.SingleOrDefault();
        }

        /// <summary>
        /// URL format: api.snooker.org/?t=6&e=[Event ID]
        /// Example: api.snooker.org/?t=6&e=398 (Shanghai Masters qualifiers)
        /// Output: Match info as JSON (two-dimensional)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Match>> GetEventMatches(int eventId)
        {
            var eventMatchesRequest = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "6")
                .Add("e", eventId.ToString())
                .BuildQuery();

            var eventMatchesResponse = await _restClient.Send<IEnumerable<Match>>(eventMatchesRequest);
            if (!eventMatchesResponse.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return eventMatchesResponse.Data;
        }

        /// <summary>
        /// URL format: api.snooker.org/?t=9&e=[Event ID]
        /// Example: api.snooker.org/?t=9&e=403 (Shanghai Masters 2015)
        /// Output: Player info as JSON (two-dimensional)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<Player>> GetEventPlayers(int eventId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "9")
                .Add("e", eventId.ToString())
                .BuildQuery();

            var response = await _restClient.Send<IEnumerable<Player>>(request);
            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// URL format: api.snooker.org/?t=12&e=[Event ID]
        /// Example: api.snooker.org/?t=12&e=403 (Shanghai Masters 2015)
        /// Output: Round info as JSON (two-dimensional)
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<RoundInfo>> GetEventRounds(int eventId)
        {
            var request = HttpRequestBuilder.Create().WithQueryParams()
                .Add("t", "12")
                .Add("e", eventId.ToString())
                .BuildQuery();
            var response = await _restClient.Send<IEnumerable<RoundInfo>>(request);

            if (!response.Success)
            {
                // log response.ErrorMessage
                return null;
            }

            return response.Data;
        }
    }
}