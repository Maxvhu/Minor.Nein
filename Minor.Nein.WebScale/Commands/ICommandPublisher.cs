﻿namespace Minor.Nein.WebScale
{
    using System;
    using System.Threading.Tasks;

    public interface ICommandPublisher : IDisposable
    {
        string QueueName { get; set; }
        Task<T> Publish<T>(DomainCommand domainCommand);
    }
}