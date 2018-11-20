﻿namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMQCommandSender : ICommandSender
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<CommandMessage>>();

        private readonly EventingBasicConsumer _consumer;
        private readonly ILogger _logger;
        private readonly string _replyQueueName;
        private bool _disposed;

        private IModel Channel { get; }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseQueueName">if responseQueueName null is given then rabbitMQ will generate a name</param>
        public RabbitMQCommandSender(RabbitMQBusContext context)
        {
            CheckDisposed();

            Channel = context.Connection.CreateModel();
            _replyQueueName = Channel.QueueDeclare().QueueName;
            _logger = NeinLogger.CreateLogger<RabbitMQCommandSender>();

            _consumer = new EventingBasicConsumer(Channel);
            _consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                {
                    return;
                }

                var body = ea.Body;
                string message = Encoding.UTF8.GetString(body);

                var commandResponse =
                        new CommandMessage(message, ea.BasicProperties.Type, ea.BasicProperties.CorrelationId);
                tcs.TrySetResult(commandResponse);
            };

            _logger.LogInformation("Created response queue with name {0}", _replyQueueName);
        }

        public Task<CommandMessage> SendCommandAsync(CommandMessage request, string queueName)
        {
            CheckDisposed();

            if (queueName == _replyQueueName)
            {
                _logger.LogWarning("The queuename {0} has the same same as the reply queue name, this should not happen"
                      , _replyQueueName);
                throw new ArgumentException($"The queuename {queueName} is the same as the reply queue name");
            }

            IBasicProperties props = Channel.CreateBasicProperties();
            string correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.Type = request.MessageType ?? "";
            props.ReplyTo = _replyQueueName;
            var message = request.EncodeMessage();

            var tcs = new TaskCompletionSource<CommandMessage>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _logger.LogTrace("Sending command message with correlation id {id} and body {body} ", correlationId
                  , request);

            Channel.BasicPublish("", queueName, false, props, message);

            Channel.BasicConsume("", true, "", false, false, null, _consumer);

            return tcs.Task;
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

        ~RabbitMQCommandSender()
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