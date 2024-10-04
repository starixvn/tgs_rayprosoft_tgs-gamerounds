using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.ApplicationServices;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class GameRoundsController : ControllerBase
{
	/// <summary>
	/// Rounds the command asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("command", Name = "Round_Command")]
	public async ValueTask<Guid> RoundCommandAsync(
		[FromServices] IMediator mediator,
		[FromBody] RoundCommandViewModel source)
		=> await mediator.Send(
			request: new RoundCommandRequest(
				Id: source.Id,
				CreatedOn: source.CreatedOn,
				GameProviderId: source.GameProviderId,
				GameProviderConfigurationId: source.GameProviderConfigurationId,
				GameId: source.GameId,
				BrandId: source.BrandId,
				PlayerCode: source.PlayerCode,
				WalletId: source.WalletId,
				CurrencyCode: source.CurrencyCode,
				Status: source.Status,
				ClientStatus: source.ClientStatus,
				ExternalRoundId: source.ExternalRoundId,
				ExternalCreatedOn: source.ExternalCreatedOn,
				ExternalClosedOn: source.ExternalClosedOn,
				Bet: source.BetAmount,
				ValidBet: source.ValidBetAmount,
				Settle: source.SettleAmount,
				Commission: source.Commission,
				Turnover: source.Turnover,
				PlayerWinLoss: source.PlayerWinLoss),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
}
