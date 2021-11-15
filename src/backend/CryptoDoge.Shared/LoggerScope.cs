using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CryptoDoge.Shared
{
    public class LoggerScope : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private bool disposedValue;
        private readonly ILogger logger;
        private readonly string callerMemberName;

        public LoggerScope(ILogger logger, [CallerMemberName] string callerMemberName = "")
        {
            logger.LogDebug($"{callerMemberName} in");
            this.logger = logger;
            this.callerMemberName = callerMemberName;
            
            stopwatch = Stopwatch.StartNew();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stopwatch.Stop();
                    logger.LogDebug($"{callerMemberName} out, took: {stopwatch.ElapsedMilliseconds} ms");
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
