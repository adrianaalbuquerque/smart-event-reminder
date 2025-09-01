using Microsoft.EntityFrameworkCore.Diagnostics;
using RabbitMQ.Client;
using SmartEventReminder.Domain.DTOs;
using System.Text;
using System.Text.Json;

namespace SmartEventReminder.Worker.RabbitMQ
{
    public class RabbitMqPublisher
    {
        private readonly string _hostname = "localhost"; // depois pode ir pra config/appsettings
        private readonly string _queueName = "fila-teste-adri";

        public async Task PublishAsync(EventDTO evento)
        {
            var factory = new ConnectionFactory() {
                HostName = _hostname,
                Port = 5672,
                UserName = "rabbitmq",
                Password = "1234",
            };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            // o que de esquisito que eu quiser colocar, é aqui
            // O rabbitmq só recebe bytes no body, então eu vou precisar serializar o objeto para Json e depois para bytes
            var json = JsonSerializer.Serialize(evento);

            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: string.Empty,
                                 routingKey: _queueName,
                                 body: body);            
        }
    }
}
