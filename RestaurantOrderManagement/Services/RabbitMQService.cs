using RabbitMQ.Client;
using System.Text;

namespace RestaurantOrderManagement.Services
{
	public class RabbitMQService
	{
		private readonly IConnection _connection;
		private readonly IModel _channel;
        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Объявляем обмен
            _channel.ExchangeDeclare(exchange: "orderExchange", type: ExchangeType.Direct);

            // Объявляем очереди
            _channel.QueueDeclare(queue: "orderQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _channel.QueueDeclare(queue: "statusQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Связываем очереди с обменом
            _channel.QueueBind(queue: "orderQueue", exchange: "orderExchange", routingKey: "order");
            _channel.QueueBind(queue: "statusQueue", exchange: "orderExchange", routingKey: "status");
        }

        public void SendMessage(string message, string type)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "orderExchange",
                                  routingKey: type,
                                  basicProperties: null,
                                  body: body);
            Console.WriteLine($"[x] Sent '{message}' with type '{type}'");
        }
        ~RabbitMQService()
		{
			_channel.Close();
			_connection.Close();
		}
	}
}
