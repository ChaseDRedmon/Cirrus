using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cirrus.API;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;

namespace Cirrus.Extensions
{
    public sealed class LoggingContext : DelegatingHandler
    {
        private readonly ILogger<LoggingContext> _logger;

        public LoggingContext(ILogger<LoggingContext>? logger = null)
        {
            _logger = logger ?? NullLogger<LoggingContext>.Instance;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = new Context().WithLogger<CirrusService>(_logger);
            request.SetPolicyExecutionContext(context);
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}