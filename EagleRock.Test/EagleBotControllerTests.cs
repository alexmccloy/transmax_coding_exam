using System;
using System.Threading.Tasks;
using EagleRock.Amqp;
using EagleRock.Cache;
using EagleRock.Controllers;
using EagleRock.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using StackExchange.Redis;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace EagleRock.Test
{
    [TestClass]
    public class EagleBotControllerTests
    {
        /// <summary>
        /// Tests that the constructor does not throw an exceptions
        /// </summary>
        [TestMethod]
        public void ValidConstructor()
        {
            _ = new EagleBotController(Mock.Create<ILogger<EagleBotController>>(),
                                       Mock.Create<ICacheInterface>(),
                                       Mock.Create<IAmqpInterface>());
        }

        /// <summary>
        /// Tests that when we receive a valid payload we store it in the cache and send it to RabbitMQ
        /// </summary>
        [TestMethod]
        public async Task ReceiveValidPayload()
        {
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var amqpInterface = Mock.Create<IAmqpInterface>();
            var controller = new EagleBotController(Mock.Create<ILogger<EagleBotController>>(),
                                                    cacheInterface,
                                                    amqpInterface);

            EagleBotPayload receivedPayload = PayloadHelpers.ValidPayload;
            EagleBotPayload cachedPayload = null;
            EagleBotPayload amqpPayload = null;

            // Check that we sent the correct payload to the Cache
            cacheInterface.Arrange(cache => cache.StorePayload(Arg.IsAny<EagleBotPayload>()))
                          .DoInstead<EagleBotPayload>(payload => cachedPayload = payload)
                          .Returns(Task.CompletedTask)
                          .OccursOnce();

            // Check that we sent the correct payload to AMQP
            amqpInterface.Arrange(amqp => amqp.PublishEvent(Arg.IsAny<EagleBotPayload>()))
                         .DoInstead<EagleBotPayload>(payload => amqpPayload = payload)
                         .OccursOnce();

            //Act
            var result = await controller.Post(receivedPayload);

            //Assert
            result.ShouldBeOfType<OkResult>();
            cachedPayload.ShouldBe(receivedPayload);
            amqpPayload.ShouldBe(receivedPayload);
            
            cacheInterface.Assert();
            amqpInterface.Assert();
        }

        /// <summary>
        /// Tests that if we cannot connect to the cache that we return the correct status
        /// </summary>
        [TestMethod]
        public async Task CacheNotAvailable()
        {
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var amqpInterface = Mock.Create<IAmqpInterface>();
            var controller = new EagleBotController(Mock.Create<ILogger<EagleBotController>>(),
                                                    cacheInterface,
                                                    amqpInterface);

            EagleBotPayload receivedPayload = PayloadHelpers.ValidPayload;

            // Check that we sent the correct payload to the Cache
            cacheInterface.Arrange(cache => cache.StorePayload(Arg.IsAny<EagleBotPayload>()))
                          .Throws(new RedisConnectionException(ConnectionFailureType.UnableToConnect, String.Empty))
                          .OccursOnce();

            // Check that we sent the correct payload to AMQP
            amqpInterface.Arrange(amqp => amqp.PublishEvent(Arg.IsAny<EagleBotPayload>()))
                         .OccursNever();

            //Act
            var result = await controller.Post(receivedPayload);

            //Assert
            result.ShouldBeOfType<StatusCodeResult>();
            (result as StatusCodeResult)?.StatusCode.ShouldBe(StatusCodes.Status503ServiceUnavailable);
            
            cacheInterface.Assert();
            amqpInterface.Assert();
        }

        /// <summary>
        /// Tests that if we encounter an exception we havent specifically caught we return an internal server error
        /// status
        /// </summary>
        [TestMethod]
        public async Task UnexpectedException()
        {
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var amqpInterface = Mock.Create<IAmqpInterface>();
            var controller = new EagleBotController(Mock.Create<ILogger<EagleBotController>>(),
                                                    cacheInterface,
                                                    amqpInterface);

            EagleBotPayload receivedPayload = PayloadHelpers.ValidPayload;

            // Check that we sent the correct payload to the Cache
            cacheInterface.Arrange(cache => cache.StorePayload(Arg.IsAny<EagleBotPayload>()))
                          .Throws(new Exception())
                          .OccursOnce();

            // Check that we sent the correct payload to AMQP
            amqpInterface.Arrange(amqp => amqp.PublishEvent(Arg.IsAny<EagleBotPayload>()))
                         .OccursNever();

            //Act
            var result = await controller.Post(receivedPayload);

            //Assert
            result.ShouldBeOfType<StatusCodeResult>();
            (result as StatusCodeResult)?.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            
            cacheInterface.Assert();
            amqpInterface.Assert();
        }
    }
}