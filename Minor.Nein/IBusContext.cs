namespace Minor.Nein
{
    using System;
    using System.Collections.Generic;

    public interface IBusContext<TConnection> : IDisposable
    {
        TConnection Connection { get; }
        string ExchangeName { get; }
        ICommandReceiver CreateCommandReceiver(string queueName);

        ICommandSender CreateCommandSender();
        IMessageReceiver CreateMessageReceiver(string queueName, IEnumerable<string> topicExpressions);

        IMessageSender CreateMessageSender();
    }
}