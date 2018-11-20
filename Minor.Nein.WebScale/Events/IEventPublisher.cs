namespace Minor.Nein.WebScale
{
    public interface IEventPublisher
    {
        void Publish(DomainEvent domainEvent);
    }
}
