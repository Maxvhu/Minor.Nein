namespace Minor.Nein.WebScale
{
    using System;
    using System.Threading.Tasks;

    public interface ICommandPublisher : IDisposable
    {
        string QueueName { get; set; }
        Task<object> Publish(DomainCommand domainCommand);
    }
}