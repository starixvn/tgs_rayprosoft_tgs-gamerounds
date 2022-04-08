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
            _logger.LogInformation("LogOn:{DateTime.Now.ToString(\"o\")} Request: {request}", DateTime.Now.ToString("o"), request);

            if (request.Content != null)
            {
                _logger.LogInformation("LogOn:{DateTime.Now.ToString(\"o\")} Request Body: {await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}", DateTime.Now.ToString("o"), await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }

            var response = await base
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("LogOn:{DateTime.Now.ToString(\"o\")} Response: {response}", DateTime.Now.ToString("o"), response);

            if (response.Content != null)
            {
                _logger.LogInformation("LogOn:{DateTime.Now.ToString(\"o\")} Response Body: {await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)}", DateTime.Now.ToString("o"), await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            }

            return response;
        }
    }
}
