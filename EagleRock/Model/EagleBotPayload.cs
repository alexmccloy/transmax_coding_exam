using System;

namespace EagleRock.Model
{
    /// <summary>
    /// Single data packet received from an EagleBot
    /// </summary>
    public record EagleBotPayload
    {
        public string EagleBotId { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public Coordinate Coordinates { get; set; }
        
        public string StreetName { get; set; }
        
        public TrafficDirection TrafficDirection { get; set; }
        
        /// <summary>
        /// Vehicles per hour on this road
        /// </summary>
        public float TrafficFlowRate { get; set; }
        
        /// <summary>
        /// Average speed of all vehicles in km/h
        /// </summary>
        public float AverageVehicleSpeed { get; set; }
    }
}