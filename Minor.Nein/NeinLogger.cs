namespace Minor.Nein
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetFunctionInformation()
        {
            string callingClass = GetFunctionClass();
            string callingFunction = GetFunctionName();

            return $"[{callingClass}]{callingFunction}:";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetFunctionName()
        {
            return new StackFrame(1).GetMethod().Name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCallerName()
        {
            return new StackFrame(2).GetMethod().Name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetFunctionClass()
        {
            string className = new StackFrame(1).GetMethod().DeclaringType?.Name;
            ;
            return className ?? "";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCallerClass()
        {
            string className = new StackFrame(2).GetMethod().DeclaringType?.Name;
            ;
            return className ?? "";
        }
    }
}