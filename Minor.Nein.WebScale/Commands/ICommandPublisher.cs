using System;
using System.Threading.Tasks;

namespace Minor.Nein.WebScale
{
    public interface ICommandPublisher : IDisposable
    {
        string QueueName { get; set; }
        Task<object> Publish(DomainCommand domainCommand);
    }
}