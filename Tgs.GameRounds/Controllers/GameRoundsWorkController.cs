using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.HandleServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class GameRoundsWorkController : ControllerBase
{
	/// <summary>
	/// Executes the asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="model">The model.</param>
	/// <returns></returns>
	[HttpPost]
	public ValueTask ExecuteAsync(
		[FromServices] ICommandService<GameRoundWorkCommand> service,
		GameRoundsWorkViewModel model)
	{
		var command = new GameRoundWorkCommand(
			model.ExternalRoundId,
			model.GameProviderId,
			model.GameId,
			model.PlayerCode,
			model.CurrencyCode,
			model.WalletId,
			model.BrandId,
			model.GameProviderConfigurationId,
			model.Status,
			model.ClientStatus,
			model.ExternalCreatedOn,
			model.ExternalClosedOn,
			model.BetAmount,
			model.ValidBetAmount,
			model.SettleAmount,
			model.Commission,
			model.Turnover,
			model.PlayerWinLoss,
			model.AdditionalData);

		return service.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);
	}
}
