namespace Minor.Nein.WebScale
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class CommandPublisher : ICommandPublisher
    {
        private readonly ILogger _logger;

        private ICommandSender Sender { get; }

        public CommandPublisher(IBusContext<IConnection> context, string queueName)
        {
            QueueName = queueName;
            Sender = context.CreateCommandSender();
            _logger = NeinLogger.CreateLogger<CommandPublisher>();
        }

        public string QueueName { get; set; }

        public async Task<T> Publish<T>(DomainCommand domainCommand)
        {
            domainCommand.TimeStamp = DateTime.Now.Ticks;
            string body = JsonConvert.SerializeObject(domainCommand);
            var commandMessage = new CommandMessage(body, domainCommand.CorrelationId);
            var task = Sender.SendCommandAsync(commandMessage, QueueName);

            if (await Task.WhenAny(task, Task.Delay(5000)) == task)
            {
                // Task completed within timeout.
                // Consider that the task may have faulted or been canceled.
                // We re-await the task so that any exceptions/cancellation is rethrown.
                CommandMessage result = await task;

                if (result.MessageType.Contains("Exception"))
                {
                    object e = null;
                    try
                    {
                        e = Activator.CreateInstance(Type.GetType(result.MessageType), result.Message);
                    }
                    catch (Exception)
                    {
                        throw new InvalidCastException(
                                $"an unknown exception occured (message {result.Message}), exception type was {result.MessageType}");
                    }

                    throw e as Exception;
                }

                var obj = JsonConvert.DeserializeObject<T>(result.Message);
                return obj;
            }

            _logger.LogWarning("MessageID {cor} did not receive a response", domainCommand.CorrelationId);
            throw new NoResponseException("Could not get a response");
        }

        public void Dispose()
        {
            Sender?.Dispose();
        }
    }
}