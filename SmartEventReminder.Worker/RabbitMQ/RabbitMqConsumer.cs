using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartEventReminder.Domain.DTOs;
using SmartEventReminder.Domain.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace SmartEventReminder.Worker.RabbitMQ
{
    public class RabbitMqConsumer
    {
        private readonly string _hostname = "localhost";
        private readonly string _queueName = "fila-teste-adri";

        public async Task Start()
        {
            var factory = new ConnectionFactory()
            { 
                HostName = _hostname,
                Port = 5672,
                UserName = "rabbitmq",
                Password = "1234",
            };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var evento = JsonSerializer.Deserialize<EventDTO>(message);
                    if (evento != null)
                    {
                        Console.WriteLine($"📩 Recebido Evento: {evento.Title} em {evento.EventDate}");

                        // dispara e-mail
                        await EnviarEmailAsync(evento);
                    }
                    Console.WriteLine($"📩 Evento Recebido: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao receber a mensagem");
                }
            };

            await channel.BasicConsumeAsync(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);            
        }
        public async Task EnviarEmailAsync(EventDTO evento)
        {
            using var smtp = new SmtpClient("smtp.gmail.com", 587) // host e porta
            {
                Credentials = new NetworkCredential("descobertasdodia035@gmail.com", "jfjfzlyojdobuume"),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress("descobertasdodia035@gmail.com"),
                Subject = $"🔔 TESTANDO A CARROÇA Lembrete: {evento.Title}",
                Body = $"Olá! O evento \"{evento.Title}\" vai acontecer em {evento.EventDate:G}.\n\nDescrição: {evento.Description}",
                IsBodyHtml = false
            };

            // aqui tu pode pegar o e-mail do usuário associado ao evento
            mail.To.Add("adrianaalbuquerque035@gmail.com");
            //mail.To.Add("andygajadhar@gmail.com");

            await smtp.SendMailAsync(mail);

            Console.WriteLine($"📨 E-mail enviado para {mail.To[0]} sobre o evento '{evento.Title}'");
        }
    }
}
