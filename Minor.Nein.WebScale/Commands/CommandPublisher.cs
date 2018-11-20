namespace Minor.Nein.WebScale
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class CommandPublisher : ICommandPublisher
    {
        private ICommandSender Sender { get; }

        public CommandPublisher(IBusContext<IConnection> context, string queueName)
        {
            QueueName = queueName;
            Sender = context.CreateCommandSender();
        }

        public string QueueName { get; set; }

        public async Task<object> Publish(DomainCommand domainCommand)
        {
            string body = JsonConvert.SerializeObject(domainCommand);
            var commandMessage = new CommandMessage(body
                  , domainCommand.ConvertResponseToType == null ? "" : domainCommand.ConvertResponseToType.ToString()
                  , domainCommand.CorrelationId);

            CommandMessage result = await Sender.SendCommandAsync(commandMessage, QueueName);

            if (result.MessageType.Contains("Exception"))
            {
                object e = null;
                try
                {
                    e = Activator.CreateInstance(Type.GetType(result.MessageType), result.Message);
                }
                catch (Exception)
                {
                    throw new Exception(
                            $"an unknown exception occured (message {result.Message}), exception type was {result.MessageType}");
                }

                throw e as Exception;
            }

            object obj = JsonConvert.DeserializeObject(result.Message, Type.GetType(result.MessageType));
            return obj;
        }

        public void Dispose()
        {
            Sender?.Dispose();
        }
    }
}