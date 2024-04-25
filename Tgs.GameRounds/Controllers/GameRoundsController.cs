using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class GameRoundsController : ControllerBase
{
	/// <summary>
	/// Rounds the command asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("command", Name = "Round_Command")]
	public ValueTask<Guid> RoundCommandAsync(
		[FromServices] IGetService<RoundCommand, Guid> service,
		RoundCommandViewModel source)
		=> service.GetAsync(
			query: new RoundCommand(
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
				PlayerWinLoss: source.PlayerWinLoss,
				AdditionalData: source.AdditionalData),
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the round by external game get asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("external-game", Name = "Round_ByExternalGame_Get")]
	public async ValueTask<RoundViewModel?> RoundByExternalGameGetAsync(
		[FromServices] IGetService<RoundByExternalGameQuery, RoundResult?> service,
		[FromQuery] RoundByExternalGameViewModel source)
	{
		var query = new RoundByExternalGameQuery(
			GameProviderId: source.GameProviderId,
			WalletId: source.WalletId,
			ExternalRoundId: source.ExternalRoundId,
			ExternalGameId: source.ExternalGameId,
			ExternalCreatedOn: source.ExternalCreatedOn);

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result is null
			? null
			: new RoundViewModel
			{
				RoundId = result.RoundId,
				GameProviderId = result.GameProviderId,
				GameProviderCode = result.GameProviderCode,
				GameProviderConfigurationId = result.GameProviderConfigurationId,
				GameId = result.GameId,
				BrandId = result.BrandId,
				PlayerCode = result.PlayerCode,
				WalletId = result.WalletId,
				CurrencyCode = result.CurrencyCode,
				Status = result.Status,
				ClientStatus = result.ClientStatus,
				ExternalRoundId = result.ExternalRoundId,
				ExternalGameId = result.ExternalGameId,
				ExternalCreatedOn = result.ExternalCreatedOn,
				ExternalClosedOn = result.ExternalClosedOn,
				BetAmount = result.BetAmount,
				ValidBetAmount = result.ValidBetAmount,
				SettleAmount = result.SettleAmount,
				Commission = result.Commission,
				Turnover = result.Turnover,
				PlayerWinLoss = result.PlayerWinLoss,
				CompanyWinLoss = -result.PlayerWinLoss,
				CreatedOn = result.CreatedOn,
				ClosedOn = result.ClosedOn,
				UpdatedOn = result.UpdatedOn,
				AdditionalData = result.AdditionalData
			};
	}

	/// <summary>
	/// Games the round by game get asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("game", Name = "Round_ByGame_Get")]
	public async ValueTask<RoundViewModel?> RoundByGameGetAsync(
		[FromServices] IGetService<RoundByGameQuery, RoundResult?> service,
		[FromQuery] RoundByGameViewModel source)
	{
		var query = new RoundByGameQuery(
			GameProviderId: source.GameProviderId,
			GameId: source.GameId,
			WalletId: source.WalletId,
			ExternalRoundId: source.ExternalRoundId,
			ExternalCreatedOn: source.ExternalCreatedOn);

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result is null
			? null
			: new RoundViewModel
			{
				RoundId = result.RoundId,
				GameProviderId = result.GameProviderId,
				GameProviderCode = result.GameProviderCode,
				GameProviderConfigurationId = result.GameProviderConfigurationId,
				GameId = result.GameId,
				BrandId = result.BrandId,
				PlayerCode = result.PlayerCode,
				WalletId = result.WalletId,
				CurrencyCode = result.CurrencyCode,
				Status = result.Status,
				ClientStatus = result.ClientStatus,
				ExternalRoundId = result.ExternalRoundId,
				ExternalGameId = result.ExternalGameId,
				ExternalCreatedOn = result.ExternalCreatedOn,
				ExternalClosedOn = result.ExternalClosedOn,
				BetAmount = result.BetAmount,
				ValidBetAmount = result.ValidBetAmount,
				SettleAmount = result.SettleAmount,
				Commission = result.Commission,
				Turnover = result.Turnover,
				PlayerWinLoss = result.PlayerWinLoss,
				CompanyWinLoss = -result.PlayerWinLoss,
				CreatedOn = result.CreatedOn,
				ClosedOn = result.ClosedOn,
				UpdatedOn = result.UpdatedOn,
				AdditionalData = result.AdditionalData
			};
	}

	/// <summary>
	/// Games the round get asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="externalCreatedOn">The external created on.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}", Name = "Round_Get")]
	public async ValueTask<RoundViewModel?> GameRoundGetAsync(
		[FromServices] IGetService<RoundQuery, RoundResult?> service,
		Guid id,
		[FromQuery] DateTimeOffset? externalCreatedOn)
	{
		var query = new RoundQuery(
			RoundId: id,
			ExternalCreatedOn: externalCreatedOn);

		var result = await service
			.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result is null
			? null
			: new RoundViewModel
			{
				RoundId = result.RoundId,
				GameProviderId = result.GameProviderId,
				GameProviderCode = result.GameProviderCode,
				GameProviderConfigurationId = result.GameProviderConfigurationId,
				GameId = result.GameId,
				BrandId = result.BrandId,
				PlayerCode = result.PlayerCode,
				WalletId = result.WalletId,
				CurrencyCode = result.CurrencyCode,
				Status = result.Status,
				ClientStatus = result.ClientStatus,
				ExternalRoundId = result.ExternalRoundId,
				ExternalGameId = result.ExternalGameId,
				ExternalCreatedOn = result.ExternalCreatedOn,
				ExternalClosedOn = result.ExternalClosedOn,
				BetAmount = result.BetAmount,
				ValidBetAmount = result.ValidBetAmount,
				SettleAmount = result.SettleAmount,
				Commission = result.Commission,
				Turnover = result.Turnover,
				PlayerWinLoss = result.PlayerWinLoss,
				CompanyWinLoss = -result.PlayerWinLoss,
				CreatedOn = result.CreatedOn,
				ClosedOn = result.ClosedOn,
				UpdatedOn = result.UpdatedOn,
				AdditionalData = result.AdditionalData
			};
	}

	/// <summary>
	/// Rounds the detail get asynchronous.
	/// </summary>
	/// <param name="repository">The repository.</param>
	/// <param name="id">The identifier.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}/getdetail", Name = "RoundDetail_Get")]
	public async ValueTask<RoundDetailViewModel?> RoundDetailGetAsync(
		[FromServices] IRoundRepository repository,
		Guid id)
	{
		var result = await repository
			.GetRoundDetailAsync(
			id: id,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return result == null
			? null
			: new RoundDetailViewModel
			{
				RoundId = result.RoundId,
				AdditionalData = result.AdditionalData
			};
	}
}
