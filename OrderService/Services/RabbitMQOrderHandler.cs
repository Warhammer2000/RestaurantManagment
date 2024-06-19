using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using OrderService.DB;
using OrderService.Models;
using System.Text.Json;

namespace OrderService.Services
{
    // Потребитель сообщений в OrderService
    public class RabbitMQOrderHandler : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQOrderHandler(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Инициализация очереди для обработки заказов
            _channel.QueueDeclare(queue: "orderQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
                    var order = JsonSerializer.Deserialize<Order>(message);
                    // Логика обработки сообщения
                    dbContext.Orders.Add(order);
                    await dbContext.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "orderQueue", autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

}