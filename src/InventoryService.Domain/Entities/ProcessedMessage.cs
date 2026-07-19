public class ProcessedMessage
{
    public Guid MessageId {get; private set;}
    public DateTime ProcessedAt {get; private set;}
    private ProcessedMessage() {}
    private ProcessedMessage(Guid messageId)
    {
        MessageId = messageId;
        ProcessedAt = DateTime.UtcNow;
    }
    public static ProcessedMessage Create(Guid messageId)
    {
        if (messageId == Guid.Empty)
        {
            throw new ArgumentException("Invalid message id");
        }
        return new ProcessedMessage(messageId);
    }
}