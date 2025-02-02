using System.Diagnostics.Metrics;
using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.Features.ServiceExtensions;
using Dotnet.Homeworks.Infrastructure.ServiceExtensions;
using Dotnet.Homeworks.MainProject.Configuration;
using Dotnet.Homeworks.MainProject.Services;
using Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;
using Dotnet.Homeworks.MainProject.ServicesExtensions.OpenTelemetry;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();

var openTelemetryConfig = builder.Configuration
    .GetSection("OpenTelemetryConfig")
    .Get<OpenTelemetryConfig>()!;
builder.Services.AddOpenTelemetry(openTelemetryConfig);

Console.WriteLine(builder.Configuration.GetConnectionString("Default"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddInfrastructure()
    .AddFeatures();

builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
builder.Services.AddSingleton<ICommunicationService, CommunicationService>();

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>()!;
builder.Services.AddMasstransitRabbitMq(rabbitMqConfig);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var getCountMeter = new Meter("Dotnet.Homeworks.GetHelloWorldMetrics");
var counter = getCountMeter.CreateCounter<int>("counter");

app.MapGet("/", () =>
{
    counter.Add(1);
    return "Hello World!";
});

app.MapControllers();

app.Run();