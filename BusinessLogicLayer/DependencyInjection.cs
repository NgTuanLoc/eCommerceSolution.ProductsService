using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IProductService, ProductService>();
        services.AddTransient<IRabbitMqPublisher, RabbitMqPublisher>();

        return services;
    }
}
