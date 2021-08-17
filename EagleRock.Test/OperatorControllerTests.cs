using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EagleRock.Cache;
using EagleRock.Controllers;
using EagleRock.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace EagleRock.Test
{
    [TestClass]
    public class OperatorControllerTests
    {
        /// <summary>
        /// Tests that the constructor does not throw an exceptions
        /// </summary>
        [TestMethod]
        public void ValidConstructor()
        {
            _ = new OperatorController(Mock.Create<ILogger<OperatorController>>(),
                                       Mock.Create<ICacheInterface>());
        }

        /// <summary>
        /// Tests that when we request a single payload that exists that we return it correctly
        /// </summary>
        [TestMethod]
        public async Task ValidGetSingleResult()
        {
            const string payloadId = "TestPayload";
            
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var controller = new OperatorController(Mock.Create<ILogger<OperatorController>>(),
                                                    cacheInterface);

            var expectedPayload = PayloadHelpers.ValidPayload with { EagleBotId = payloadId };

            cacheInterface.Arrange(cache => cache.GetPayload(payloadId))
                          .Returns(Task.FromResult(expectedPayload))
                          .OccursOnce();

            cacheInterface.Arrange(cache => cache.GetAllPayloads())
                          .OccursNever();

            //Act
            var result = await controller.Get(payloadId);

            //Assert
            result.ShouldBeOfType<OkObjectResult>();
            (result as OkObjectResult)?.Value.ShouldBe(expectedPayload);
            
            cacheInterface.Assert();
        }
        
        /// <summary>
        /// Tests that when we request a single payload that does not exist that we return the correct error
        /// </summary>
        [TestMethod]
        public async Task InvalidGetSingleResult()
        {
            const string payloadId = "TestPayload";
            
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var controller = new OperatorController(Mock.Create<ILogger<OperatorController>>(),
                                                    cacheInterface);

            cacheInterface.Arrange(cache => cache.GetPayload(payloadId))
                          .Throws(new ArgumentNullException())
                          .OccursOnce();

            cacheInterface.Arrange(cache => cache.GetAllPayloads())
                          .OccursNever();

            //Act
            var result = await controller.Get(payloadId);

            //Assert
            result.ShouldBeOfType<NotFoundResult>();
            
            cacheInterface.Assert();
        } 
        
        /// <summary>
        /// Tests that when we request all payloads that we return them correctly
        /// </summary>
        [TestMethod]
        public async Task ValidGetAllResults()
        {
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var controller = new OperatorController(Mock.Create<ILogger<OperatorController>>(),
                                                    cacheInterface);

            var expectedPayload1 = PayloadHelpers.ValidPayload with { EagleBotId = "Payload1" };
            var expectedPayload2 = PayloadHelpers.ValidPayload with { EagleBotId = "Payload2" };
            var expectedPayload3 = PayloadHelpers.ValidPayload with { EagleBotId = "Payload3" };

            cacheInterface.Arrange(cache => cache.GetPayload(Arg.AnyString))
                          .OccursNever();

            cacheInterface.Arrange(cache => cache.GetAllPayloads())
                          .Returns(Task.FromResult( (new List<EagleBotPayload>()
                                           {
                                               expectedPayload1, expectedPayload2, expectedPayload3
                                           }) as IEnumerable<EagleBotPayload>))
                          .OccursOnce();

            //Act
            var result = await controller.Get();

            //Assert
            result.ShouldBeOfType<OkObjectResult>();
            var payloads = (result as OkObjectResult)?.Value as IEnumerable<EagleBotPayload>;
            payloads?.ShouldNotBeNull();
            payloads.ShouldContain(expectedPayload1);
            payloads.ShouldContain(expectedPayload2);
            payloads.ShouldContain(expectedPayload3);
            
            cacheInterface.Assert();
        }
        
        /// <summary>
        /// Tests that when we request all payloads and the cache is empty that we return an empty list
        /// </summary>
        [TestMethod]
        public async Task GetAllResultsEmptyCache()
        {
            //Arrange
            var cacheInterface = Mock.Create<ICacheInterface>();
            var controller = new OperatorController(Mock.Create<ILogger<OperatorController>>(),
                                                    cacheInterface);


            cacheInterface.Arrange(cache => cache.GetPayload(Arg.AnyString))
                          .OccursNever();

            cacheInterface.Arrange(cache => cache.GetAllPayloads())
                          .Returns(Task.FromResult(Array.Empty<EagleBotPayload>() as IEnumerable<EagleBotPayload>))
                          .OccursOnce();

            //Act
            var result = await controller.Get();

            //Assert
            result.ShouldBeOfType<OkObjectResult>();
            var payloads = (result as OkObjectResult)?.Value as IEnumerable<EagleBotPayload>;
            payloads?.ShouldNotBeNull();
            payloads.Count().ShouldBe(0);
            
            cacheInterface.Assert();
        }
    }
}