using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BetSnooker.Configuration;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BetSnooker.Services
{
    /// <summary>
    /// Current api.snooker.org rate limit is 10 requests per minute.
    /// </summary>
    public class SnookerApiService : ISnookerApiService
    {
        private readonly IFlurlRequest _snookerApiRequest;
        private readonly ILogger _logger;

        public SnookerApiService(ISettingsProvider settingsProvider, ILogger<SnookerApiService> logger)
        {
            _snookerApiRequest = settingsProvider.SnookerApiUrl.WithHeader("X-Requested-By", settingsProvider.RequestedByHeader);
            _logger = logger;
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
            try
            {
                _logger.LogDebug("api.snooker.org: getting events");
                var response = await _snookerApiRequest.SetQueryParam("t", 5).SetQueryParam("s", season).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                _logger.LogDebug("api.snooker.org: events successfully retrieved");
                var result = JsonConvert.DeserializeObject<IEnumerable<Event>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                _logger.LogDebug("api.snooker.org: getting event");
                var response = await _snookerApiRequest.SetQueryParam("e", eventId).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                _logger.LogDebug("api.snooker.org: event successfully retrieved");
                var result = JsonConvert.DeserializeObject<IEnumerable<Event>>(responseContent);
                return result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("e", eventId).SetQueryParam("r", roundId).SetQueryParam("n", matchNumber)
                    .GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<Match>>(responseContent);
                return result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("p", playerId).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<Player>>(responseContent);
                return result.SingleOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("t", 6).SetQueryParam("e", eventId).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<Match>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// URL format: api.snooker.org/?t=7
        /// Example: api.snooker.org/?t=7
        /// Output: Match info as JSON (two-dimensional)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Match>> GetOngoingMatches()
        {
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("t", 7).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<Match>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("t", 9).SetQueryParam("e", eventId).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<Player>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
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
            try
            {
                var response = await _snookerApiRequest.SetQueryParam("t", 12).SetQueryParam("e", eventId).GetAsync();
                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    _logger.LogError(response.ResponseMessage.ReasonPhrase);
                    return null;
                }

                var responseContent = await response.ResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<IEnumerable<RoundInfo>>(responseContent);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}