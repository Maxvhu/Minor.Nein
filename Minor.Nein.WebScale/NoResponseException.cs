namespace Minor.Nein.WebScale
{
    using System;

    public class NoResponseException : Exception
    {
        public NoResponseException()
        {
        }

        public NoResponseException(string message) : base(message)
        {
        }
    }
}