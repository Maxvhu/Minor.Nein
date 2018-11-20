namespace Minor.Nein.WebScale
{
    using System;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class EventPublisher : IEventPublisher, IDisposable
    {
        private IMessageSender Sender { get; }

        public EventPublisher(IBusContext<IConnection> context)
        {
            Sender = context.CreateMessageSender();
        }

        public void Dispose()
        {
            Sender?.Dispose();
        }

        public void Publish(DomainEvent domainEvent)
        {
            string body = JsonConvert.SerializeObject(domainEvent);
            var eventMessage = new EventMessage(domainEvent.RoutingKey, body);
            Sender.SendMessage(eventMessage);
        }
    }
}