namespace Minor.Nein.RabbitMQBus
{
    using System;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class RabbitMQMessageSender : IMessageSender
    {
        private readonly ILogger _log;
        private bool disposed;
        public string ExchangeName { get; }
        private IModel Channel { get; }

        public RabbitMQMessageSender(RabbitMQBusContext context)
        {
            Channel = context.Connection.CreateModel();
            ExchangeName = context.ExchangeName;
            _log = NeinLogger.CreateLogger<RabbitMQMessageSender>();
        }

        public void SendMessage(IEventMessage message)
        {
            CheckDisposed();

            _log.LogTrace($"Sending message to routing key {message.RoutingKey ?? ""}");

            var body = message.EncodeMessage();

            IBasicProperties basicProperties = Channel.CreateBasicProperties();
            basicProperties.Timestamp =
                    new AmqpTimestamp(message.Timestamp == 0 ? DateTime.Now.Ticks : message.Timestamp);
            basicProperties.CorrelationId = message.CorrelationId ?? Guid.NewGuid().ToString();
            basicProperties.Type = message.EventType ?? "";

            Channel.BasicPublish(ExchangeName, message.RoutingKey, false, basicProperties, body);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Channel?.Dispose();
            }

            disposed = true;
        }

        ~RabbitMQMessageSender()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion
    }
}