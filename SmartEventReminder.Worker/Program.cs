using Microsoft.EntityFrameworkCore;
using Quartz;
using SmartEventReminder.Domain;
using SmartEventReminder.Worker;
using SmartEventReminder.Worker.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var consumer = new RabbitMqConsumer();
_ = consumer.Start();

builder.Services.AddScoped<RabbitMqPublisher>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // é interessante criar uma chave única para identificar o job, isso é bom quando se deseja associar múltiplas triggers a um mesmo job
    var jobkey = new JobKey("SendEvent");
    q.AddJob<Jobs>(opts => opts.WithIdentity(jobkey));

    // Trigger
    q.AddTrigger(opts => opts
    .ForJob(jobkey)
    .WithIdentity("SendEvent-trigger")
    .WithCronSchedule("0/30 * * * * ?")
    //.WithCronSchedule("0 0/10 * * * ?") // tinha deixado 10  * * * * mas ele tava rodando 10 segundos de cada minuto
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
