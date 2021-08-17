using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EagleRock.Cache
{
    /// <summary>
    /// Helper methods to store and retrieve data from Redis
    /// </summary>
    public static class RedisCacheExtensions
    {
        /// <summary>
        /// Serialises an object to JSON and stores it in a Redis cache.
        /// </summary>
        /// <param name="cache">The cache where the data will be stored</param>
        /// <param name="recordId">The key to store the value under</param>
        /// <param name="data">The actual object being stored in cache</param>
        /// <param name="absoluteExpireTime">Time before the object expires.
        /// If this is not set data will expire after 1 day</param>
        /// <param name="unusedExpireTime">Time before the object expires if it is not used</param>
        public static async Task StoreDataAsJsonAsync<T>(this IDistributedCache cache,
                                                         string recordId,
                                                         T data,
                                                         TimeSpan? absoluteExpireTime = null,
                                                         TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromDays(1),
                SlidingExpiration = unusedExpireTime
            };

            var jsonData = JsonConvert.SerializeObject(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        /// <summary>
        /// Retrieves a value from Redis (in JSON) and deserialises it.
        /// </summary>
        /// <param name="cache">The cache to retrieve data from</param>
        /// <param name="recordId">The key of the value to retrieve</param>
        /// <returns></returns>
        public static async Task<T> LoadDataFromJsonAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}