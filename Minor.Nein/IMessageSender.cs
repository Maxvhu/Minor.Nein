namespace Minor.Nein
{
    using System;

    public interface IMessageSender : IDisposable
    {
        void SendMessage(IEventMessage message);
    }
}