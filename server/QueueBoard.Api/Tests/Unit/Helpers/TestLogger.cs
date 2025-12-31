using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace QueueBoard.Api.Tests.Unit.Helpers
{
    public class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new List<string>();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var msg = formatter(state, exception);
                Messages.Add(msg ?? string.Empty);
            }
            catch
            {
                // swallow formatting errors in tests
            }
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            public void Dispose() { }
        }
    }
}
