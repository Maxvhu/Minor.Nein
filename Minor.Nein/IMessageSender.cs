using System;

namespace Minor.Nein
{
    public interface IMessageSender : IDisposable
    {
        void SendMessage(IEventMessage message);
    }
}
