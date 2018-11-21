namespace Minor.Nein.RabbitMQBus
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMQCommandReceiver : ICommandReceiver
    {
        private readonly ILogger _log;

        private IModel Channel { get; }

        public RabbitMQCommandReceiver(RabbitMQBusContext context, string queueName)
        {
            _log = NeinLogger.CreateLogger<RabbitMQCommandReceiver>();

            Channel = context.Connection.CreateModel();
            QueueName = queueName;
        }

        public string QueueName { get; }

        public void DeclareCommandQueue()
        {
            CheckDisposed();

            _log.LogInformation("Declared a queue {0} for commands", QueueName);
            Channel.QueueDeclare(QueueName, false, false, false, null);
            Channel.BasicQos(0, 1, false);
        }

        public void StartReceivingCommands(CommandReceivedCallback callback)
        {
            CheckDisposed();

            var consumer = new EventingBasicConsumer(Channel);
            Channel.BasicConsume(QueueName, false, "", false, false, null, consumer);

            consumer.Received += (s, ea) =>
            {
                var body = ea.Body;
                IBasicProperties props = ea.BasicProperties;
                IBasicProperties replyProps = Channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                replyProps.Type = props.Type;

                string message = Encoding.UTF8.GetString(body);
                CommandMessage response = null;
                try
                {
                    response = callback(new CommandMessage(message, props.Type, props.CorrelationId));
                    replyProps.Type = response.MessageType.ToString();
                }
                catch (Exception e)
                {
                    Exception ie = e.InnerException;
                    response = new CommandMessage(ie.Message, ie.GetType().ToString(), props.CorrelationId);
                    replyProps.Type = ie.GetType().ToString();
                }
                finally
                {
                    Channel.BasicPublish("", props.ReplyTo, false, replyProps, response?.EncodeMessage());

                    Channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _log.LogInformation("Started listening for commands on queue {0} ", QueueName);
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

        ~RabbitMQCommandReceiver()
        {
            Dispose(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                var logger = NeinLogger.CreateLogger<RabbitMQCommandReceiver>();
                logger.LogCritical($"{NeinLogger.GetFunctionInformation()} Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");

                throw new ObjectDisposedException(GetType().FullName, $"Trying to call a function in a disposed object! (Function: {NeinLogger.GetCallerName()})");
            }
        }

        #endregion
    }
}