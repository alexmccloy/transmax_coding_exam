using System;
using EagleRock.Model;
using EagleRock.Model.Validation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace EagleRock.Test
{
    [TestClass]
    public class EagleBotPayloadValidatorTests
    {
        /// <summary>
        /// Tests a Valid EagleBot Payload passes validation
        /// </summary>
        [TestMethod]
        public void ValidEagleBotPayload()
        {
            var payload = PayloadHelpers.ValidPayload;
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.Errors.Count.ShouldBe(0);
        }

        /// <summary>
        /// Tests that we get a validation error when the EagleBotId is empty
        /// </summary>
        [TestMethod]
        public void InvalidEagleBotId()
        {
            var payload = PayloadHelpers.ValidPayload with { EagleBotId = String.Empty };
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.ShouldHaveValidationErrorFor(x => x.EagleBotId);
        }
        
        /// <summary>
        /// Tests that we get a validation error when the Coordinates are out of range
        /// </summary>
        [TestMethod]
        public void InvalidCoordinates()
        {
            var payload = PayloadHelpers.ValidPayload with
            {
                Coordinates = new Coordinate()
                {
                    Latitude = -1000,
                    Longitude = -1000
                }
            };
            
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.ShouldHaveValidationErrorFor(x => x.Coordinates.Latitude);
            result.ShouldHaveValidationErrorFor(x => x.Coordinates.Longitude);
        }
        
        /// <summary>
        /// Tests that we get a validation error when the Street Name is empty
        /// </summary>
        [TestMethod]
        public void InvalidStreetName()
        {
            var payload = PayloadHelpers.ValidPayload with { StreetName = String.Empty };
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.ShouldHaveValidationErrorFor(x => x.StreetName);
        }
        
        /// <summary>
        /// Tests that we get a validation error when the TrafficFlowRate is negative
        /// </summary>
        [TestMethod]
        public void InvalidTrafficFlowRate()
        {
            var payload = PayloadHelpers.ValidPayload with { TrafficFlowRate = -1 };
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.ShouldHaveValidationErrorFor(x => x.TrafficFlowRate);
        }
        
        /// <summary>
        /// Tests that we get a validation error when the Average Vehicle Speed is negative
        /// </summary>
        [TestMethod]
        public void InvalidAverageVehicleSpeed()
        {
            var payload = PayloadHelpers.ValidPayload with {AverageVehicleSpeed = -1 };
            var result = new EagleBotPayloadValidator().TestValidate(payload);
            result.ShouldHaveValidationErrorFor(x => x.AverageVehicleSpeed);
        }
    }
}