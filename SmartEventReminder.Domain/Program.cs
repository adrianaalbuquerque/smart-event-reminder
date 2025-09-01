using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartEventReminder.Domain;

// Cria o host (genérico, não web)
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Registra o DbContext usando connection string do appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build();

// Teste simples: criar um scope e usar o contexto
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("Conectado no banco com sucesso!");
    // aqui já pode rodar consultas linq
}
