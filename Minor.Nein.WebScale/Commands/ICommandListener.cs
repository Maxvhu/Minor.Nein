namespace Minor.Nein.WebScale
{
    using System;
    using RabbitMQ.Client;

    public interface ICommandListener : IDisposable
    {
        string QueueName { get; }

        void DeclareQueue(IBusContext<IConnection> context);
        CommandMessage Handle(CommandMessage commandMessage);
        void StartListening(IMicroserviceHost host);
    }
}