using System.Text.Json;


public class OutboxMessage
{
    public long Id {get; private set;}
    public Guid MessageId {get; private set;}
    public string EventType {get; private set;} = string.Empty;
    public string Payload {get; private set;} = string.Empty;
    public DateTime CreatedAt {get; private set;}
    public DateTime? ProcessedAt {get; private set;}
    public bool IsProcessed {get; private set;}

    private OutboxMessage(string eventType, string payload)
    {
        MessageId = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        IsProcessed = true;
    }

    public static OutboxMessage Create(IIntegrationEvent evt)
    {
        string? eventType = evt.GetType().Name;
        return new OutboxMessage(eventType != null ? eventType : string.Empty, JsonSerializer.Serialize((object)evt));
    }
}