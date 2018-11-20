using System;

namespace Minor.Nein.WebScale
{
    class NoSuitableConstructorException : Exception
    {
        public NoSuitableConstructorException()
        {
        }

        public NoSuitableConstructorException(string message) : base(message)
        {
        }

    }
}
