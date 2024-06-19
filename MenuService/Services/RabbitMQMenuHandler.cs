using MenuService.DB;
using MenuService.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace MenuService.Services
{
    public class RabbitMQMenuHandler : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQMenuHandler(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory() { HostName = "localhost" };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "menuQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
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
                    var dbContext = scope.ServiceProvider.GetRequiredService<MenuContext>();
                    var menuItem = JsonSerializer.Deserialize<MenuItem>(message);
                    await dbContext.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "menuQueue", autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
        private async Task ProcessMenuMessage(string message)
        {
            Console.WriteLine($"Processing menu message: {message}");
            try
            {
                var menuItem = JsonSerializer.Deserialize<MenuItem>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MenuContext>();
                    await dbContext.SaveChangesAsync();
                }

                Console.WriteLine("Menu item saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process menu message: {ex.Message}");
            }
        }
    }
}
