using System.Collections.Generic;
using System.Threading.Tasks;
using EagleRock.Model;

namespace EagleRock.Cache
{
    public interface ICacheInterface
    {
        Task StorePayload(EagleBotPayload payload);
        Task<EagleBotPayload> GetPayload(string id);
        Task<IEnumerable<EagleBotPayload>> GetAllPayloads();
    }
}