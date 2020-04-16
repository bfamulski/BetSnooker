using System;
using System.Collections.Generic;

namespace BetSnooker.Services
{
    public interface ICacheService<T>
    {
        void Save(List<T> items);
        List<T> Get();
        bool Exists();
    }

    public class CacheService<T> : ICacheService<T>
    {
        private static List<T> _cachedItems = new List<T>();
        private static DateTime? _lastSaveTime;

        private readonly TimeSpan _expirationTime;

        public CacheService(TimeSpan expirationTime)
        {
            _expirationTime = expirationTime;
        }

        public void Save(List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            lock (_cachedItems)
            {
                _cachedItems = items;
                _lastSaveTime = DateTime.Now;
            }
        }

        public List<T> Get()
        {
            if (!_lastSaveTime.HasValue || _lastSaveTime.Value + _expirationTime < DateTime.Now)
            {
                return null;
            }

            return _cachedItems;
        }

        public bool Exists()
        {
            if (!_lastSaveTime.HasValue || _lastSaveTime.Value + _expirationTime < DateTime.Now)
            {
                return false;
            }

            if (_cachedItems == null || _cachedItems.Count == 0)
            {
                return false;
            }

            return true;
        }
    }
}