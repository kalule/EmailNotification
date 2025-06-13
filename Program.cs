using Common.Events.Extensions;
using Common.Events.Infrastructure;
using Common.Events.Interfaces;
using Common.Messaging.Interfaces;
using EmailNotification.Models;
using Serilog;
using Common.Messaging.Models;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/email-notification-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Plug Serilog into the ASP.NET Core pipeline
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings")); // load SMTP settings from appsettings.json
builder.Services.AddEventBus(builder.Configuration); // this should load EventBus settings from appsettings.json
builder.Services.AddScoped<IEventBusIntegrationEventHandler<Common.Events.Models.EmailRequestedEvent>, EmailRequestedEventHandler>();


builder.Services.AddHostedService<EventConsumerBackgroundService>();
builder.Services.AddLogging(logging => logging.AddConsole());
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
