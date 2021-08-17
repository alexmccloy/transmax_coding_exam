using System.Collections.Generic;
using System.Threading.Tasks;
using EagleRock.Model;

namespace EagleRock.Cache
{
    /// <summary>
    /// Interface to store and load values from a cache
    /// </summary>
    public interface ICacheInterface
    {
        /// <summary>
        /// Stores an EagleBot Payload into the cache. If this value already exists it will be overwritten
        /// </summary>
        /// <param name="payload">The data to store in cache</param>
        Task StorePayload(EagleBotPayload payload);
        
        /// <summary>
        /// Retrieves a payload from the cache matching the given id
        /// </summary>
        /// <param name="id">The id of the <see cref="EagleBotPayload"/> to retrieve</param>
        /// <returns>An <see cref="EagleBotPayload"/> with the matching id</returns>
        Task<EagleBotPayload> GetPayload(string id);
        
        /// <summary>
        /// Retrieves all payloads from the cache that contain <see cref="EagleBotPayload"/>s
        /// </summary>
        /// <returns>All the <see cref="EagleBotPayload"/>s that are available in the cache</returns>
        Task<IEnumerable<EagleBotPayload>> GetAllPayloads();
    }
}