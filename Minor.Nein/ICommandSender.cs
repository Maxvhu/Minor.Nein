using System;
using System.Threading.Tasks;

namespace Minor.Nein
{
    public interface ICommandSender : IDisposable
    {
        Task<CommandMessage> SendCommandAsync(CommandMessage request, string queueName);
    }
}
