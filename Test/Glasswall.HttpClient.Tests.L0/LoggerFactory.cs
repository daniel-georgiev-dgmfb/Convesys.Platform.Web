using Microsoft.Extensions.Logging;

namespace Twiligth.HttpClient.Tests.L0
{
    internal class LoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger();
        }

        public void Dispose()
        {

        }
    }
}