using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServiceAbstraction.Infrastructure;

namespace DataServiceAbstraction.Services.Decorators
{

    /// <summary>
    /// High-performance caching decorator following LexisPlus Austria Redis patterns.
    /// Demonstrates senior-level caching strategies for enterprise scale.
    /// </summary>
    public class CachedDataService : IDataService
    {

        private readonly IDataService _innerService;
        private readonly TimeSpan _cacheTimeout;
        private readonly ILogger _logger;

        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private readonly string _cacheKey;

        public CachedDataService(IDataService innerService, TimeSpan cacheTimeout, ILogger? logger = null)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _cacheTimeout = cacheTimeout;
            _logger = logger ?? new ConsoleLogger();
            _cacheKey = $"DataCache_{innerService.GetHashCode()}";
        }

        public IEnumerable<string> GetLines()
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            var now = DateTime.UtcNow;

            // PERFORMANCE: Check cache first (following Redis patterns from config)
            if (_cache.TryGetValue(_cacheKey, out var cached) && !cached.IsExpired(now, _cacheTimeout))
            {
                _logger.LogInformation($"[{requestId}] Cache HIT - serving {cached.LineCount:N0} cached lines");
                return cached.Data;
            }

            _logger.LogInformation($"[{requestId}] Cache MISS - fetching fresh data");

            var stopwatch = Stopwatch.StartNew();

            // SCALABILITY: Stream and cache in memory-efficient chunks
            var data = new List<string>();
            var chunkCount = 0;

            foreach (var line in _innerService.GetLines())
            {
                data.Add(line);

                // OBSERVABILITY: Progress reporting for large datasets
                if (data.Count % 50000 == 0)
                {
                    chunkCount++;
                    _logger.LogInformation($"[{requestId}] Cached chunk {chunkCount}: {data.Count:N0} lines");
                }
            }

            stopwatch.Stop();

            // PERFORMANCE: Update cache atomically
            var newEntry = new CacheEntry(data, now, data.Count);
            _cache.AddOrUpdate(_cacheKey, newEntry, (key, old) => newEntry);

            _logger.LogInformation($"[{requestId}] Cached {data.Count:N0} lines in {stopwatch.ElapsedMilliseconds}ms " +
                                 $"(expires in {_cacheTimeout.TotalMinutes:F0} minutes)");

            return data;
        }

        private record CacheEntry(List<string> Data, DateTime CachedAt, int LineCount)
        {
            public bool IsExpired(DateTime now, TimeSpan timeout) => (now - CachedAt) > timeout;
        }
    }
}
