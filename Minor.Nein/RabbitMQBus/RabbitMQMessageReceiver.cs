namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMQMessageReceiver : IMessageReceiver
    {
        private readonly ILogger _log;
        private bool _disposed;
        private bool _listening;
        private bool _queueDeclared;
        public string ExchangeName { get; }
        private IModel Channel { get; }

        public RabbitMQMessageReceiver(RabbitMQBusContext context, string queueName
                                     , IEnumerable<string> topicExpressions)
        {
            _log = NeinLogger.CreateLogger<RabbitMQMessageReceiver>();

            QueueName = queueName;
            TopicExpressions = topicExpressions;
            Channel = context.Connection.CreateModel();

            ExchangeName = context.ExchangeName;

            _listening = false;
            _queueDeclared = false;
        }

        public string QueueName { get; }
        public IEnumerable<string> TopicExpressions { get; }

        public void DeclareQueue()
        {
            CheckDisposed();
            if (_queueDeclared)
            {
                _log.LogWarning($"Trying to declare a queue ({QueueName}) twice");
                throw new BusConfigurationException("Can't declare the queue multiple times");
            }

            Channel.QueueDeclare(QueueName, true, false, false);
            if (TopicExpressions == null || !TopicExpressions.Any())
            {
                _log.LogInformation($"Queue {QueueName} is now listening on default routingKey");

                Channel.QueueBind(QueueName, ExchangeName, "", null);
            }
            else
            {
                foreach (string bindingKey in TopicExpressions)
                {
                    _log.LogInformation($"Queue {QueueName} is now listening on {bindingKey}");

                    Channel.QueueBind(QueueName, ExchangeName, bindingKey, null);
                }
            }

            _queueDeclared = true;
        }

        public void StartReceivingMessages(EventMessageReceivedCallback callback)
        {
            CheckDisposed();
            if (_listening)
            {
                _log.LogWarning($"Trying to start listening to ({QueueName}) twice");
                throw new BusConfigurationException("Can't start listening multiple times");
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                string message = Encoding.UTF8.GetString(body);

                var eventMessage = new EventMessage(ea.RoutingKey, message, ea.BasicProperties.Type
                      , ea.BasicProperties.Timestamp.UnixTime, ea.BasicProperties.CorrelationId);
                callback(eventMessage);
            };

            Channel.BasicConsume(QueueName, true, "", false, false, null, consumer);
            _listening = true;
        }

        #region Dispose

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

        ~RabbitMQMessageReceiver()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion
    }
}