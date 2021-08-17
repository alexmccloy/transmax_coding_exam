using System;
using System.Threading.Tasks;
using EagleRock.Auth;
using EagleRock.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EagleRock.Controllers
{
    /// <summary>
    /// Interface for other backend systems to view EagleBot data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OperatorController : ControllerBase
    {
        private readonly ILogger<OperatorController> _logger;
        private readonly ICacheInterface _cacheInterface;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logging interface</param>
        /// <param name="cacheInterface">Interface to cache the most recent EagleBot payload</param>
        public OperatorController(ILogger<OperatorController> logger, ICacheInterface cacheInterface)
        {
            _logger = logger;
            _cacheInterface = cacheInterface;
        }

        /// <summary>
        /// Gets the current dataset for a single EagleBot. If the EagleBot Id is null then the data for all EagleBots
        /// will be returned instead.
        /// </summary>
        /// <param name="eagleBotId">The Id of the EagleBot to retrieve data for, or null for all data</param>
        [HttpGet]
        [ApiKeyAuth]
        public async Task<IActionResult> Get(string eagleBotId = null)
        {
            try
            {
                if (eagleBotId is null)
                {
                    return new OkObjectResult(await _cacheInterface.GetAllPayloads());
                }
                else
                {
                    return new OkObjectResult(await _cacheInterface.GetPayload(eagleBotId));
                }
            }
            catch (RedisConnectionException e)
            {
                _logger.LogError("Failed to retrieve EagleBot payload - a connection to the cache could not be established", e);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogWarning($"Received request to retrieve EagleBot payload with id {eagleBotId} that does not exist in cache", e);
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError($"An unexpected error occured while trying to retrieve an EagleBot payload with Id {eagleBotId}: {e.GetType()}{e.Message}", e);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}