namespace Minor.Nein.WebScale
{
    using System;

    internal class NoSuitableConstructorException : Exception
    {
        public NoSuitableConstructorException()
        {
        }

        public NoSuitableConstructorException(string message) : base(message)
        {
        }
    }
}