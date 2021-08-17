using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using EagleRock.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EagleRock.Amqp
{
    /// <summary>
    /// Interface to allow this application to publish messages to RabbitMQ. 
    /// </summary>
    /// <remarks>
    /// Currently this implementation does not attempt to re-connect to RabbitMQ if the connection is lost for any reason.
    /// </remarks>
    public class RabbitMqInterface :  IAmqpInterface, IHostedService, IDisposable
    {
        private readonly ILogger<RabbitMqInterface> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "EagleRock";
        private const string RoutingKey = "eaglerock.eaglebot.events";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logging interface for this class</param>
        /// <param name="connectionFactory">Factory to produce connections to RabbitMQ</param>
        /// <param name="config">Configuration containing the RabbitMq connection string</param>
        public RabbitMqInterface(ILogger<RabbitMqInterface> logger, IConnectionFactory connectionFactory, IConfiguration config)
        {
            _logger = logger;
            
            _connectionFactory = connectionFactory;
            _connectionFactory.Uri = new Uri(config.GetConnectionString("RabbitMq"));
        }
        
       protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _channel?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Repeatedly try to connect to rabbit (Note: in a production app this would not be a good solution since
            // this blocks the app from starting until we are connected)
            while (true)
            {
                try
                {
                    _connection = _connectionFactory.CreateConnection();
                    break;
                }
                catch (BrokerUnreachableException)
                {
                    // We failed to connect, try again in 1 second
                    await Task.Delay(1000, cancellationToken);
                }
            }
            
            _channel = _connection.CreateModel();
            
            // Declare our exchange if it doesnt already exist
            try
            {
                _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
            }
            catch (OperationInterruptedException)
            {
                //Ignore this - it means the exchange already exists
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            
            return Task.CompletedTask;
        }

        public void PublishEvent(EagleBotPayload payload)
        {
            var jsonMessage = JsonConvert.SerializeObject(payload);
            byte[] message = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
            
            _channel.BasicPublish(ExchangeName, RoutingKey, null, message);
        }

 
    }
}