namespace BusinessLogicLayer.RabbitMQ;

public interface IRabbitMqPublisher
{
    void Publish<T>(string routingKey, T message);
}