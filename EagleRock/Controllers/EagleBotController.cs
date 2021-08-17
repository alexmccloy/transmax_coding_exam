using System;
using System.Threading.Tasks;
using EagleRock.Amqp;
using EagleRock.Auth;
using EagleRock.Cache;
using EagleRock.Model;
using EagleRock.Model.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EagleRock.Controllers
{
    /// <summary>
    /// Interface to allow EagleBots to submit their data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EagleBotController : ControllerBase
    {
        private readonly ILogger<EagleBotController> _logger;
        private readonly ICacheInterface _cacheInterface;
        private readonly IAmqpInterface _amqpInterface;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logging interface</param>
        /// <param name="cacheInterface">Interface to cache the most recent EagleBot payload</param>
        /// <param name="amqpInterface">Interface to publish events to when we receive a payload</param>
        public EagleBotController(ILogger<EagleBotController> logger, 
                                  ICacheInterface cacheInterface,
                                  IAmqpInterface amqpInterface)
        {
            _logger = logger;
            _cacheInterface = cacheInterface;
            _amqpInterface = amqpInterface;
        }

        /// <summary>
        /// Stores the payload from an EagleBot into the cache and database. This payload is validated by
        /// <see cref="EagleBotPayloadValidator"/>
        /// </summary>
        /// <param name="payload">A single data packet from an eagle bot</param>
        /// <returns>OK if data successfully stored in cache, otherwise returns the appropriate error</returns>
        [HttpPost]
        [ApiKeyAuth]
        public async Task<IActionResult> Post([FromBody] EagleBotPayload payload)
        {
            try
            {
                await _cacheInterface.StorePayload(payload);
                _logger.LogDebug($"Successfully cached payload from EagleBot with Id: {payload.EagleBotId}");
                
                _amqpInterface.PublishEvent(payload);
                _logger.LogDebug($"Successfully published event to AMQP from EagleBot with Id: {payload.EagleBotId}");
            }
            catch (RedisConnectionException e)
            {
                _logger.LogError("Failed to cache EagleBot payload - a connection to the cache could not be established", e);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception e)
            {
                _logger.LogError("An unexpected error occured while trying to cache an EagleBot payload", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
            return Ok();
        }
    }
}