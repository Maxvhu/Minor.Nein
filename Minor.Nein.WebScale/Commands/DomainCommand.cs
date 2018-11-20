namespace Minor.Nein.WebScale
{
    using System;

    public abstract class DomainCommand
    {
        public Type ConvertResponseToType { get; set; }
        public string CorrelationId { get; set; }
        public string QueueName { get; set; }
        public long TimeStamp { get; set; }
    }
}