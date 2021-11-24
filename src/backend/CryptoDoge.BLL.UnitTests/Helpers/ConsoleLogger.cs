using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDoge.Services.UnitTests.Helpers
{
    public class ConsoleLogger<T> : ILogger<T>
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
