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
                var requestContent = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Request Body: {requestContent}", requestContent);
            }

            var response = await base
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("Response: {response}", response);

            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Response Body: {ResponseContent}", responseContent);
            }

            return response;
        }
    }
}
