using System;
using System.Threading.Tasks;
using Twilight.Kernel.Logging;

namespace Twilight.Platform.Web.HttpClient
{
    public class HttpClientLogger : IEventLogger<Twiligth.Platform.Web.HttpClient.HttpClient>
    {
        public void Log<TState>(SeverityLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(@"Log entry:/r/n.SeverityLevel: {0}, Exception:{1}", logLevel, exception);
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Guid transactionId, string message)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, string message)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Guid transactionId, Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public class BackchanelLogger : IEventLogger<Twiligth.Platform.Web.HttpClient.BackchannelCertificateValidator>
    {
        public void Log<TState>(SeverityLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine(@"Log entry:/r/n.SeverityLevel: {0}, Exception:{1}", logLevel, exception);
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Guid transactionId, string message)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, string message)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Guid transactionId, Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Log(SeverityLevel level, EventId eventId, Type eventSource, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
