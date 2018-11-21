namespace Minor.Nein
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class BusConfigurationException : Exception
    {
        public BusConfigurationException(string message) : base(message)
        {
        }
    }
}