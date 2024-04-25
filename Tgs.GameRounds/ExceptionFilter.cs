using Microsoft.AspNetCore.Mvc.Filters;

namespace Tgs.GameRounds;

public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
{
	public void OnException(ExceptionContext context)
	{
		logger.LogError(context.Exception, "exception procedure");
	}
}
