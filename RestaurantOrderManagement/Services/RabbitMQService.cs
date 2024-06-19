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
        private readonly string _exchangeName;

        public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "defaultExchange";
                _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);

                InitializeQueue(configuration, "OrderQueueName", "order");
                InitializeQueue(configuration, "StatusQueueName", "status");
                InitializeQueue(configuration, "MenuQueueName", "menu");

                _logger.LogInformation("RabbitMQService initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQService.");
                Dispose();
                throw;
            }
        }

        private void InitializeQueue(IConfiguration configuration, string queueConfigName, string routingKey)
        {
            var queueName = configuration[$"RabbitMQ:{queueConfigName}"] ?? $"{routingKey}Queue";
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKey);
        }

        public void SendMessage(string message, string routingKey)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                _logger.LogInformation($"Sent message '{message}' with routing key '{routingKey}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send message '{message}' with routing key '{routingKey}'");
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
