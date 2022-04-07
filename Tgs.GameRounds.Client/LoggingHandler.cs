using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tgs.GameRounds.Client
{
    internal class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
            : base()
            => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Request: {request}", request);

            if (request.Content != null)
            {
                _logger.LogInformation("Request Body: {await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}", await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }

            var response = await base
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("Response: {response}", response);

            if (response.Content != null)
            {
                _logger.LogInformation("Response Body: {await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}", await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }

            return response;
        }
    }
}
