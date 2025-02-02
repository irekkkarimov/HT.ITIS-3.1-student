using Dotnet.Homeworks.MainProject.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        return services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
                configurator.Host(new Uri($"rabbitmq://{rabbitConfiguration.Hostname}"), hostConfigurator =>
                {
                    hostConfigurator.Username(rabbitConfiguration.Username);
                    hostConfigurator.Password(rabbitConfiguration.Password);
                });
            });
        });
    }
}