using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RestaurantOrderManagement.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"]
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                var exchange = configuration["RabbitMQ:ExchangeName"] ?? "orderExchange";
                _channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                var orderQueue = configuration["RabbitMQ:OrderQueueName"] ?? "orderQueue";
                var statusQueue = configuration["RabbitMQ:StatusQueueName"] ?? "statusQueue";

                _channel.QueueDeclare(queue: orderQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueDeclare(queue: statusQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

                _channel.QueueBind(queue: orderQueue, exchange: exchange, routingKey: "order");
                _channel.QueueBind(queue: statusQueue, exchange: exchange, routingKey: "status");

                _logger.LogInformation("RabbitMQService initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQService.");
                Dispose();
                throw;
            }
        }

        public void SendMessage(string message, string type)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                var exchange = "orderExchange"; 

                _channel.BasicPublish(exchange: exchange, routingKey: type, basicProperties: null, body: body);
                _logger.LogInformation($"Sent message '{message}' with type '{type}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send message '{message}' with type '{type}'");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("RabbitMQService disposed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispose RabbitMQService.");
            }
        }
    }
}
