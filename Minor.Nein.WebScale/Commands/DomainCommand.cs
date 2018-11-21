namespace Minor.Nein.WebScale
{
    public abstract class DomainCommand
    {
        public string CorrelationId { get; set; }
        public string QueueName { get; set; }
        public long TimeStamp { get; set; }
    }
}