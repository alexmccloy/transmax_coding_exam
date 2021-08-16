using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EagleRock.Model;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace EagleRock.Cache
{
    public class RedisInterface : ICacheInterface
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private const string _keyPrefix = "EagleBot_";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache">The cache that this class will use</param>
        /// <param name="connectionMultiplexer"></param>
        public RedisInterface(IDistributedCache cache,
                              IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        /// <summary>
        /// Stores a single payload in the Redis cache. Payload will be converted to Json and stored as a string.
        /// </summary>
        /// <param name="payload">The payload to store</param>
        public async Task StorePayload(EagleBotPayload payload)
        {
            string payloadKey = $"{_keyPrefix}{payload.EagleBotId}";
            await _cache.StoreDataAsJsonAsync(payloadKey, payload);
        }

        /// <summary>
        /// Retrieves a single payload from the Redis cache. Assumes that payload was stored as Json.
        /// </summary>
        /// <param name="id">The Id of the EagleBot whose data we are retrieving</param>
        public Task<EagleBotPayload> GetPayload(string id)
        {
            string payloadKey = $"{_keyPrefix}{id}";
            return _cache.LoadDataFromJsonAsync<EagleBotPayload>(payloadKey);
        }

        /// <summary>
        /// Retrieves all values from the Redis cache that match the EagleBot key prefix.
        /// </summary>
        /// <remarks>
        /// This implementation is not efficient as at retrieves the values one at a time. In the future this should be
        /// replaced with methods from the RedisExtensions library.
        /// See https://imperugo.gitbook.io/stackexchange-redis-extensions/usage/work-with-multiple-items
        /// </remarks>
        public async Task<IEnumerable<EagleBotPayload>> GetAllPayloads()
        {
            var result = new List<EagleBotPayload>();
            
            foreach (var key in GetKeys($"{_keyPrefix}*"))
            {
                result.Add(await _cache.LoadDataFromJsonAsync<EagleBotPayload>(key));
            }

            return result;
        }

        /// <summary>
        /// Gets all keys in Redis that match the pattern
        /// </summary>
        private IEnumerable<string> GetKeys(string pattern)
        {
            return _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First())
                                         .Keys(pattern: Startup.CacheKeyPrefix + pattern)
                                         .Select(redisKey =>
                                         {
                                             // The string currently has the Redis Instance name in front of it, we need
                                             // to remove that as the Redis library will re-add it.
                                             var key = redisKey.ToString();
                                             if (key.StartsWith(Startup.CacheKeyPrefix))
                                             {
                                                 key = key.Substring(Startup.CacheKeyPrefix.Length);
                                             }

                                             return key;
                                         });
            

        }
    }
}