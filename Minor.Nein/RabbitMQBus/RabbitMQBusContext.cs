namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;

    public class RabbitMQBusContext : IBusContext<IConnection>
    {
        private readonly ILogger _log;

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
            return new RabbitMQMessageSender(this);
        }

        public IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicExpressions)
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Message Receiver");
            return new RabbitMQMessageReceiver(this, queueName, topicExpressions);
        }

        public ICommandSender CreateCommandSender()
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Command Sender");
            return new RabbitMQCommandSender(this);
        }

        public ICommandReceiver CreateCommandReceiver(string queueName)
        {
            CheckDisposed();

            _log.LogInformation("Creating new RabbitMQ Command receiver");
            return new RabbitMQCommandReceiver(this, queueName);
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
                Connection?.Dispose();
            }

            _disposed = true;
        }

        ~RabbitMQBusContext()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                var logger = NeinLogger.CreateLogger<RabbitMQBusContext>();
                logger.LogCritical($"{NeinLogger.GetFunctionInformation()} Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");

                throw new ObjectDisposedException(GetType().FullName, $"Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");
            }
        }

        #endregion
    }
}