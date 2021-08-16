using System;
using System.Threading.Tasks;
using EagleRock.Database;
using EagleRock.Model;
using EagleRock.Model.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EagleRock.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EagleBotController : ControllerBase
    {
        private readonly ILogger<EagleBotController> _logger;
        private readonly ICacheInterface _cacheInterface;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logging interface</param>
        /// <param name="cacheInterface">Interface to cache the most recent EagleBot payload</param>
        public EagleBotController(ILogger<EagleBotController> logger, 
                                  ICacheInterface cacheInterface)
        {
            _logger = logger;
            _cacheInterface = cacheInterface;
        }

        /// <summary>
        /// Stores the payload from an EagleBot into the cache and database. This payload is validated by
        /// <see cref="EagleBotPayloadValidator"/>
        /// </summary>
        /// <param name="payload">A single data packet from an eagle bot</param>
        /// <returns>OK if data successfully stored in cache, otherwise returns the appropriate error</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EagleBotPayload payload)
        {
            try
            {
                await _cacheInterface.StorePayload(payload);
                _logger.LogDebug($"Successfully cached payload from EagleBot with Id: {payload.EagleBotId}");
            }
            catch (Exception e)
            {
                //TODO catch more specific exceptions
                _logger.LogError("Failed to cache EagleBot payload", e);
            }
            return Ok();
        }
    }
}