using Quartz;
using SmartEventReminder.Domain;
using SmartEventReminder.Domain.DTOs;
using SmartEventReminder.Worker.RabbitMQ;

namespace SmartEventReminder.Worker
{
    public class Jobs : IJob
    {
        private readonly ILogger<Jobs> _logger;
        private readonly AppDbContext _context;
        private readonly RabbitMqPublisher _publisher;

        public Jobs(ILogger<Jobs> logger, AppDbContext context, RabbitMqPublisher publisher)
        {
            _logger = logger;
            _context = context;
            _publisher = publisher;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Enviando aviso do lembrete em: {date}", DateTime.UtcNow);

            var agora = DateTime.UtcNow;

            // Checa eventos que vão acontecer na próxima 1h
            var eventosProximaHora = _context.Events
                .Where(e => e.EventDate >= agora.AddMinutes(10)
                         && e.EventDate <= agora.AddHours(1)
                         && (e.ultimanotificacao1h == null))
                .ToList();

           foreach (var evento in eventosProximaHora)
            {
                // chave e valor
                var mensagem = new EventDTO
                {
                    Title = evento.Title,
                    Description = evento.Description,
                    EventDate  = evento.EventDate.ToUniversalTime(),
                    Notificacao = "1h antes"
                };

                await _publisher.PublishAsync(mensagem);
                _logger.LogInformation($"Evento '{evento.Title}' publicado para RabbitMQ (1h antes)");

                evento.ultimanotificacao1h = agora;
            }

            // Checa eventos que vão acontecer nos próximos 10 minutos
            var eventos10Min = _context.Events
                .Where(e => e.EventDate >= agora
                         && e.EventDate <= agora.AddMinutes(10)
                         && (e.ultimanotificacao10m == null))
                .ToList();

            foreach (var evento in eventos10Min)
            { // Alerta final
                var mensagem = new EventDTO
                {
                    Title = evento.Title,
                    Description = evento.Description,
                    EventDate = evento.EventDate.ToUniversalTime(),
                    Notificacao = "10min antes"
                };

                await _publisher.PublishAsync(mensagem);

                _logger.LogInformation($"Evento '{evento.Title}' publicado para RabbitMQ (10min antes)");


                // Marca que já notifica 10 min antes
                evento.ultimanotificacao10m = agora;
            }
            _context.SaveChanges();
        }

    }
}
