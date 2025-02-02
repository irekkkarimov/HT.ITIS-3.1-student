using System.Reflection;
using Dotnet.Homeworks.Features.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Features.ServiceExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(AssemblyReference.Assembly);
        });

        return services;
    }
}