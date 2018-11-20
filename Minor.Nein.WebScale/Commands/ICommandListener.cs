using RabbitMQ.Client;
using System;

namespace Minor.Nein.WebScale
{
    public interface ICommandListener : IDisposable
    {
        string QueueName { get; }

        void DeclareQueue(IBusContext<IConnection> context);
        CommandMessage Handle(CommandMessage commandMessage);
        void StartListening(IMicroserviceHost host);
    }
}