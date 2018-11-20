using System;

namespace Minor.Nein.WebScale
{
    public interface IMicroserviceHost : IDisposable
    {
        IServiceProvider Provider { get; }

        object CreateInstanceOfType(Type type);
        void StartListening();
    }
}
