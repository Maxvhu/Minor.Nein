namespace Minor.Nein
{
    using System;
    using System.Threading.Tasks;

    public interface ICommandSender : IDisposable
    {
        Task<CommandMessage> SendCommandAsync(CommandMessage request, string queueName);
    }
}