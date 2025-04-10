namespace BusinessLogicLayer.RabbitMQ;

public record ProductNameUpdateMessage(Guid ProductId, string NewName);