namespace Minor.Nein.WebScale
{
    using System;

    public interface IMicroserviceHost : IDisposable
    {
        IServiceProvider Provider { get; }

        object CreateInstanceOfType(Type type);
        void StartListening();
    }
}