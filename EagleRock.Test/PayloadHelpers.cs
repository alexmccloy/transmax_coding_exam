using System;
using EagleRock.Model;

namespace EagleRock.Test
{
    public class PayloadHelpers
    {
        public static readonly EagleBotPayload ValidPayload = new EagleBotPayload()
        {
            EagleBotId = "Test-Id",
            Coordinates = new Coordinate()
            {
                Latitude = -27.0,
                Longitude = 153.0
            },
            Timestamp = DateTime.Now,
            StreetName = "Kingsford Smith Drive",
            TrafficDirection = TrafficDirection.Inbound,
            AverageVehicleSpeed = 100,
            TrafficFlowRate = 100
        };
    }
}