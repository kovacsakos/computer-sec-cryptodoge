using Microsoft.Extensions.Logging;

namespace CryptoDoge.Server.Tests.Helpers
{
    internal class ConsoleLogger<T> : ILogger<T>
    {
        public System.IDisposable BeginScope<TState>(TState state) => default;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            System.Exception exception, System.Func<TState, System.Exception, string> formatter)
        {
            System.Console.WriteLine($"{logLevel} {eventId} {formatter(state, exception)}");
        }
    }
}
