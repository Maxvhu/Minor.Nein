namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class RabbitMQMessageSender : IMessageSender
    {
        private readonly ILogger _log;

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

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Channel?.Dispose();
            }

            _disposed = true;
        }

        ~RabbitMQMessageSender()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                var logger = NeinLogger.CreateLogger<RabbitMQMessageSender>();
                logger.LogCritical($"{NeinLogger.GetFunctionInformation()} Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");

                throw new ObjectDisposedException(GetType().FullName, $"Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");
            }
        }

        #endregion
    }
}