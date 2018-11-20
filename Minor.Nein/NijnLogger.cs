using Microsoft.Extensions.Logging;

namespace Minor.Nein
{
    public static class NeinLogger
    {
        private static ILoggerFactory DefaultFactory { get; set; }  = new LoggerFactory();
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILogger CreateLogger<T>()
        {
            if (LoggerFactory == null) return DefaultFactory.CreateLogger<T>();
            return LoggerFactory.CreateLogger<T>();
        }
    }
}
