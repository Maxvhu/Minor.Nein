namespace Minor.Nein
{
    using Microsoft.Extensions.Logging;

    public static class NeinLogger
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        private static ILoggerFactory DefaultFactory { get; } = new LoggerFactory();

        public static ILogger CreateLogger<T>()
        {
            if (LoggerFactory == null)
            {
                return DefaultFactory.CreateLogger<T>();
            }

            return LoggerFactory.CreateLogger<T>();
        }
    }
}