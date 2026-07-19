public interface IIntegrationEvent
{
    public Guid MessageId { get;}
    public DateTime OccuredAt { get; }
}