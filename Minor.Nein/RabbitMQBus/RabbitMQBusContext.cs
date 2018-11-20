namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class RabbitMQBusContext : IBusContext<IConnection>
    {
        private readonly ILogger _log;
        private bool disposed;

        public RabbitMQBusContext(IConnection connection, string exchangeName)
        {
            _log = NeinLogger.CreateLogger<RabbitMQBusContext>();
            Connection = connection;
            ExchangeName = exchangeName;
        }

        public IConnection Connection { get; }
        public string ExchangeName { get; }

        public IMessageSender CreateMessageSender()
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Message Sender");
            var messageSender = new RabbitMQMessageSender(this);
            return messageSender;
        }

        public IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicExpressions)
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Message Receiver");
            var messageReciever = new RabbitMQMessageReceiver(this, queueName, topicExpressions);
            return messageReciever;
        }

        public ICommandSender CreateCommandSender()
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Command Sender");
            var commandSender = new RabbitMQCommandSender(this);
            return commandSender;
        }

        public ICommandReceiver CreateCommandReceiver(string queueName)
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Command receiver");
            var commandReceiver = new RabbitMQCommandReceiver(this, queueName);
            return commandReceiver;
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
                Connection?.Dispose();
            }

            disposed = true;
        }

        ~RabbitMQBusContext()
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