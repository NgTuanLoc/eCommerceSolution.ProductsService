using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            string connectionTemplate = config.GetConnectionString("DefaultConnection")!;
            string connectionString = connectionTemplate
                                        .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
                                        .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT"))
                                        .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"))
                                        .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
                                        .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER"));


            options.UseMySQL(connectionString);
        });

        services.AddScoped<IProductsRepository, ProductsRepository>();

        return services;
    }
}
