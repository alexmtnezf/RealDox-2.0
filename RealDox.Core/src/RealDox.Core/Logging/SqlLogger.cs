using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace RealDox.Core.Logging
{
    public class SqlLogger : ILogger
    {
        private static readonly string _logFilePath = @"C:\temp\DatabaseLog.sql";

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!Directory.Exists(@"C:\temp"))
                    Directory.CreateDirectory(@"C:\temp");
            var message = string.Format(
                "\n\n--{0}\n{1}",
                DateTime.Now,
                formatter(state, exception));//.Replace(", [", ",\n  ["));

            File.AppendAllText(_logFilePath, message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
