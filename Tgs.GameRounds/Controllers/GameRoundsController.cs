using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.ViewModels;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class GameRoundsController : ControllerBase
{
	/// <summary>
	/// Rounds the command asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="serialNumberGenerator">The serial number generator.</param>
	/// <param name="timeProvider">The time provider.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("command", Name = "Round_Command")]
	public async ValueTask<Guid> RoundCommandAsync(
		[FromServices] IMediator mediator,
		[FromServices] ISerialNumberGenerator serialNumberGenerator,
		[FromServices] TimeProvider timeProvider,
		[FromBody] RoundCommandViewModel source)
	{
		var id = source.Id ?? await serialNumberGenerator.GenerateAsync(HttpContext.RequestAborted).ConfigureAwait(false);
		var createdOn = source.CreatedOn ?? timeProvider.GetUtcNow();
		return await mediator.Send(
			request: new RoundCommandRequest(
				Id: id,
				CreatedOn: createdOn,
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
				PlayerWinLoss: source.PlayerWinLoss,
				AdditionalData: source.AdditionalData),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Rounds the get external game asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("external-game", Name = "Round_Get_ExternalGame")]
	public async ValueTask<RoundViewModel?> RoundGetExternalGameAsync(
		[FromServices] IMediator mediator,
		[FromQuery] RoundByExternalGameViewModel source)
	{
		var response = await mediator.Send(
			request: new RoundGetExternalGameRequest(
				GameProviderId: source.GameProviderId,
				WalletId: source.WalletId,
				ExternalRoundId: source.ExternalRoundId,
				ExternalGameId: source.ExternalGameId,
				ExternalCreatedOn: source.ExternalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new RoundViewModel
			{
				RoundId = response.RoundId,
				GameProviderId = response.GameProviderId,
				GameProviderCode = response.GameProviderCode,
				GameProviderConfigurationId = response.GameProviderConfigurationId,
				GameId = response.GameId,
				BrandId = response.BrandId,
				PlayerCode = response.PlayerCode,
				WalletId = response.WalletId,
				CurrencyCode = response.CurrencyCode,
				Status = response.Status,
				ClientStatus = response.ClientStatus,
				ExternalRoundId = response.ExternalRoundId,
				ExternalGameId = response.ExternalGameId,
				ExternalCreatedOn = response.ExternalCreatedOn,
				ExternalClosedOn = response.ExternalClosedOn,
				BetAmount = response.BetAmount,
				ValidBetAmount = response.ValidBetAmount,
				SettleAmount = response.SettleAmount,
				Commission = response.Commission,
				PlayerWinLoss = response.PlayerWinLoss,
				CompanyWinLoss = -response.PlayerWinLoss,
				CreatedOn = response.CreatedOn,
				ClosedOn = response.ClosedOn,
				UpdatedOn = response.UpdatedOn,
				AdditionalData = response.AdditionalData
			};
	}

	/// <summary>
	/// Rounds the get game asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpGet("game", Name = "Round_Get_Game")]
	public async ValueTask<RoundViewModel?> RoundGetGameAsync(
		[FromServices] IMediator mediator,
		[FromQuery] RoundByGameViewModel source)
	{
		var response = await mediator.Send(
			request: new RoundGetGameRequest(
				GameProviderId: source.GameProviderId,
				GameId: source.GameId,
				WalletId: source.WalletId,
				ExternalRoundId: source.ExternalRoundId,
				ExternalCreatedOn: source.ExternalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new RoundViewModel
			{
				RoundId = response.RoundId,
				GameProviderId = response.GameProviderId,
				GameProviderCode = response.GameProviderCode,
				GameProviderConfigurationId = response.GameProviderConfigurationId,
				GameId = response.GameId,
				BrandId = response.BrandId,
				PlayerCode = response.PlayerCode,
				WalletId = response.WalletId,
				CurrencyCode = response.CurrencyCode,
				Status = response.Status,
				ClientStatus = response.ClientStatus,
				ExternalRoundId = response.ExternalRoundId,
				ExternalGameId = response.ExternalGameId,
				ExternalCreatedOn = response.ExternalCreatedOn,
				ExternalClosedOn = response.ExternalClosedOn,
				BetAmount = response.BetAmount,
				ValidBetAmount = response.ValidBetAmount,
				SettleAmount = response.SettleAmount,
				Commission = response.Commission,
				PlayerWinLoss = response.PlayerWinLoss,
				CompanyWinLoss = -response.PlayerWinLoss,
				CreatedOn = response.CreatedOn,
				ClosedOn = response.ClosedOn,
				UpdatedOn = response.UpdatedOn,
				AdditionalData = response.AdditionalData
			};
	}

	/// <summary>
	/// Rounds the get asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="externalCreatedOn">The external created on.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}", Name = "Round_Get")]
	public async ValueTask<RoundViewModel?> RoundGetAsync(
		[FromServices] IMediator mediator,
		Guid id,
		[FromQuery] DateTimeOffset? externalCreatedOn)
	{
		var response = await mediator.Send(
			request: new RoundGetRequest(
				RoundId: id,
				ExternalCreatedOn: externalCreatedOn),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new RoundViewModel
			{
				RoundId = response.RoundId,
				GameProviderId = response.GameProviderId,
				GameProviderCode = response.GameProviderCode,
				GameProviderConfigurationId = response.GameProviderConfigurationId,
				GameId = response.GameId,
				BrandId = response.BrandId,
				PlayerCode = response.PlayerCode,
				WalletId = response.WalletId,
				CurrencyCode = response.CurrencyCode,
				Status = response.Status,
				ClientStatus = response.ClientStatus,
				ExternalRoundId = response.ExternalRoundId,
				ExternalGameId = response.ExternalGameId,
				ExternalCreatedOn = response.ExternalCreatedOn,
				ExternalClosedOn = response.ExternalClosedOn,
				BetAmount = response.BetAmount,
				ValidBetAmount = response.ValidBetAmount,
				SettleAmount = response.SettleAmount,
				Commission = response.Commission,
				PlayerWinLoss = response.PlayerWinLoss,
				CompanyWinLoss = -response.PlayerWinLoss,
				CreatedOn = response.CreatedOn,
				ClosedOn = response.ClosedOn,
				UpdatedOn = response.UpdatedOn,
				AdditionalData = response.AdditionalData
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
		var result = await repository.GetRoundDetailAsync(
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
