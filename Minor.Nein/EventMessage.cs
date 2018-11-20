namespace Minor.Nein
{
    using System.Text;

    public class EventMessage : IEventMessage
    {
        public EventMessage(string routingKey, string message, string eventType = null, long timestamp = 0
                          , string correlationId = null)
        {
            RoutingKey = routingKey;
            Message = message;
            EventType = eventType;
            Timestamp = timestamp;
            CorrelationId = correlationId;
        }

        public string RoutingKey { get; }
        public string Message { get; }
        public string EventType { get; }
        public long Timestamp { get; }
        public string CorrelationId { get; }

        public byte[] EncodeMessage()
        {
            return Encoding.UTF8.GetBytes(Message);
        }
    }
}