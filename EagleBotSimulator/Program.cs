using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EagleBotSimulator
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Random _random = new Random();
        
        static async Task Main(string[] args)
        {
            var count = int.Parse(Environment.GetEnvironmentVariable("EagleBotSimulator_Count") ?? "5");
            var tasks = new List<Task>();
            
            _httpClient.DefaultRequestHeaders.Add("ApiKey", "SampleApiKey12345");

            // Create tasks to loop and send random data forever
            for (int i = 0; i < count; i++)
            {
                int i1 = i;
                tasks.Add(Task.Run(async () =>
                {
                    string id = $"eaglebot-{i1}";
                    while (true)
                    {
                        // Send random data forever, and wait 5-15 secs between each
                        await SendRandomData(id);
                        await Task.Delay(_random.Next(5000, 15000));
                    }
                }));
            }

            // Wait forever
            await Task.WhenAll(tasks);
        }

        private static async Task SendRandomData(string id)
        {
            try
            {
                var body = new StringContent(CreateRandomData(id), Encoding.Default, "application/json");
                // await _httpClient.PostAsync("http://localhost:5000/api/eaglebot", stringContent);
                await _httpClient.PostAsync("http://eaglerock:80/api/eaglebot", body);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to write to {id}: {e.Message}");
            }
        }

        private static string CreateRandomData(string id)
        {
            return JsonConvert.SerializeObject(new {
                eagleBotId= id,
                timestamp= DateTime.Now,
                coordinates= new {
                    latitude= GetRandomDouble(-90,90),
                    longitude= GetRandomDouble(-180,180)
                },
                streetName= "Kingsford Smith Drive",
                trafficDirection= "Inbound",
                trafficFlowRate= GetRandomDouble(0, 1000),
                averageVehicleSpeed= GetRandomDouble(0, 200)
            });
        }
        
        private static double GetRandomDouble(double minimum, double maximum) => 
            _random.NextDouble() * (maximum - minimum) + minimum;
    }
}