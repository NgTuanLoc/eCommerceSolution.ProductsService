using System.Text;
using System.Text.Json;
using BusinessLogicLayer.ServiceContracts;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace BusinessLogicLayer.RabbitMQ;

public class RabbitMqPublisher:IRabbitMqPublisher, IDisposable
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        var hostName = _configuration["RabbitMQ:HostName"]!;
        var userName = _configuration["RabbitMQ:UserName"]!;
        var password = _configuration["RabbitMQ:Password"]!;
        var port = _configuration["RabbitMQ:Port"]!;

        var connectionFactory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = int.Parse(port)
        };
        
        _connection = connectionFactory.CreateConnection();
        this._channel = _connection.CreateModel();
    }
    public async void Publish<T>(string routingKey, T message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageBodyInBytes = Encoding.UTF8.GetBytes(messageJson);
        var exchangeName = _configuration["RabbitMQ:Products:Exchange"];

        // Create an exchange
        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

        // Publish messages
        _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: messageBodyInBytes);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}