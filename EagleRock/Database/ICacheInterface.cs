using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EagleRock.Model;

namespace EagleRock.Database
{
    public interface ICacheInterface
    {
        Task StorePayload(EagleBotPayload payload);
        Task<EagleBotPayload> GetPayload(string id);
        Task<IEnumerable<EagleBotPayload>> GetAllPayloads();
    }
    
    public class RedisInterface : ICacheInterface
    {
        public Task StorePayload(EagleBotPayload payload)
        {
            throw new System.NotImplementedException();
        }

        public Task<EagleBotPayload> GetPayload(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<EagleBotPayload>> GetAllPayloads()
        {
            throw new System.NotImplementedException();
        }
    }
}