using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using BetSnooker.Models.API;
using BetSnooker.Services.Interfaces;

namespace BetSnooker.Services
{
    public class SnookerApiEntityWithExpiration<T>
    {
        public T Entity { get; set; }
        public DateTime CachedAt { get; set; }
    }

    public class SnookerCacheService : ISnookerCacheService
    {
        private const string eventCacheKey = "event";
        private const string matchesCacheKey = "matches";
        private const string playersCacheKey = "players";
        private const string roundsCacheKey = "rounds";

        private readonly TimeSpan _eventExpiration = TimeSpan.FromDays(1);
        private readonly TimeSpan _matchesExpiration = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _playersExpiration = TimeSpan.FromDays(1);
        private readonly TimeSpan _roundsExpiration = TimeSpan.FromHours(1);

        private ISnookerApiService _snookerApiService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;

        public SnookerCacheService(ISnookerApiService snookerApiService, IMemoryCache memoryCache, ILogger<SnookerCacheService> logger)
        {
            _snookerApiService = snookerApiService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<Event> GetEvent(int eventId)
        {
            return await GetEntity(eventId, eventCacheKey, _eventExpiration, eventId => _snookerApiService.GetEvent(eventId));
        }

        public async Task<IEnumerable<Match>> GetEventMatches(int eventId)
        {
            return await GetEntity(eventId, matchesCacheKey, _matchesExpiration, eventId => _snookerApiService.GetEventMatches(eventId));
        }

        public async Task<IEnumerable<Player>> GetEventPlayers(int eventId)
        {
            return await GetEntity(eventId, playersCacheKey, _playersExpiration, eventId => _snookerApiService.GetEventPlayers(eventId));
        }

        public async Task<IEnumerable<RoundInfo>> GetEventRounds(int eventId)
        {
            return await GetEntity(eventId, roundsCacheKey, _roundsExpiration, eventId => _snookerApiService.GetEventRounds(eventId));
        }

        public async Task<IEnumerable<Event>> GetEvents(int season)
        {
            throw new NotImplementedException();
        }

        public async Task<Match> GetMatch(int eventId, int roundId, int matchNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Match>> GetOngoingMatches()
        {
            throw new NotImplementedException();
        }

        public async Task<Player> GetPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        private async Task<T> GetEntity<T>(int eventId, string cacheKey, TimeSpan entityCacheExpiration, Func<int, Task<T>> apiFunction)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out SnookerApiEntityWithExpiration<T> entityValue))
            {
                entityValue = await GetEntityInternal(eventId);
            }
            else
            {
                if ((DateTime.Now - entityValue.CachedAt) > entityCacheExpiration)
                {
                    _ = Task.Run(async () =>
                    {
                        entityValue = await GetEntityInternal(eventId);
                    });
                }
            }

            return entityValue.Entity;

            async Task<SnookerApiEntityWithExpiration<T>> GetEntityInternal(int eventId)
            {
                try
                {
                    var entity = await apiFunction(eventId);
                    entityValue = new SnookerApiEntityWithExpiration<T> { Entity = entity, CachedAt = DateTime.Now };
                    _memoryCache.Set(cacheKey, entityValue);
                    return entityValue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }
    }
}
