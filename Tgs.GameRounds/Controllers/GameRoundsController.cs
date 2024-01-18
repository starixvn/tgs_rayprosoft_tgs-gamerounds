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
    /// Games the round by external game get asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    [HttpGet("external-game", Name = "GameRound_ByExternalGame_Get")]
    public async ValueTask<GameRoundViewModel?> GameRoundByExternalGameGetAsync(
        [FromServices] IGetService<GameRoundByExternalGameQuery, GameRoundResult?> service,
        [FromQuery] GameRoundByExternalGameViewModel source)
    {
        var query = new GameRoundByExternalGameQuery(
            GameProviderId: source.GameProviderId,
            ExternalRoundId: source.ExternalRoundId,
            ExternalGameId: source.ExternalGameId,
            WalletId: source.WalletId);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result is null
            ? null
            : new GameRoundViewModel
            {
                RoundId = result.RoundId,
                ExternalRoundId = result.ExternalRoundId,
                GameProviderId = result.GameProviderId,
                GameProviderCode = result.GameProviderCode,
                PlayerCode = result.PlayerCode,
                GameId = result.GameId,
                ExternalGameId = result.ExternalGameId,
                CurrencyCode = result.CurrencyCode,
                Status = result.Status,
                GameProviderConfigurationId = result.GameProviderConfigurationId,
                WalletId = result.WalletId,
                CreatedOn = result.CreatedOn,
                ClosedOn = result.ClosedOn,
                UpdatedOn = result.UpdatedOn,
                BrandId = result.BrandId,
                ExternalCreatedOn = result.ExternalCreatedOn,
                ExternalClosedOn = result.ExternalClosedOn,
                BetAmount = result.BetAmount,
                ValidBetAmount = result.ValidBetAmount,
                SettleAmount = result.SettleAmount,
                Commission = result.Commission,
                Turnover = result.Turnover,
                CompanyWinLoss = result.CompanyWinLoss,
                TieBetAdjustment = result.TieBetAdjustment,
                ClientStatus = result.ClientStatus,
                ReopenTurnoverAdjustment = result.ReopenTurnoverAdjustment,
                ReopenedOn = result.ReopenedOn,
                EffectiveBettingPercentage = result.EffectiveBettingPercentage,
                AdditionalData = result.AdditionalData
            };
    }

    /// <summary>
    /// Games the round by game get asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    [HttpGet("game", Name = "GameRound_ByGame_Get")]
    public async ValueTask<GameRoundViewModel?> GameRoundByGameGetAsync(
        [FromServices] IGetService<GameRoundByGameQuery, GameRoundResult?> service,
        [FromQuery] GameRoundByGameViewModel source)
    {
        var query = new GameRoundByGameQuery(
            GameProviderId: source.GameProviderId,
            ExternalRoundId: source.ExternalRoundId,
            GameId: source.GameId,
            WalletId: source.WalletId);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result is null
            ? null
            : new GameRoundViewModel
            {
                RoundId = result.RoundId,
                ExternalRoundId = result.ExternalRoundId,
                GameProviderId = result.GameProviderId,
                GameProviderCode = result.GameProviderCode,
                PlayerCode = result.PlayerCode,
                GameId = result.GameId,
                ExternalGameId = result.ExternalGameId,
                CurrencyCode = result.CurrencyCode,
                Status = result.Status,
                GameProviderConfigurationId = result.GameProviderConfigurationId,
                WalletId = result.WalletId,
                CreatedOn = result.CreatedOn,
                ClosedOn = result.ClosedOn,
                UpdatedOn = result.UpdatedOn,
                BrandId = result.BrandId,
                ExternalCreatedOn = result.ExternalCreatedOn,
                ExternalClosedOn = result.ExternalClosedOn,
                BetAmount = result.BetAmount,
                ValidBetAmount = result.ValidBetAmount,
                SettleAmount = result.SettleAmount,
                Commission = result.Commission,
                Turnover = result.Turnover,
                CompanyWinLoss = result.CompanyWinLoss,
                TieBetAdjustment = result.TieBetAdjustment,
                ClientStatus = result.ClientStatus,
                ReopenTurnoverAdjustment = result.ReopenTurnoverAdjustment,
                ReopenedOn = result.ReopenedOn,
                EffectiveBettingPercentage = result.EffectiveBettingPercentage,
                AdditionalData = result.AdditionalData
            };
    }

    /// <summary>
    /// Gets the game round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="id">The identifier.</param>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Get
    ///         /gamerounds/018919ab-2ece-0000-0001-07e7be4ca7ee
    /// </remarks>
    [HttpGet("{id:guid}", Name = "GameRound_Get")]
    public async ValueTask<GameRoundViewModel?> GameRoundGetAsync(
        [FromServices] IGetService<GameRoundQuery, GameRoundResult?> service,
        Guid id)
    {
        var query = new GameRoundQuery(
            RoundId: id);

        var result = await service
            .GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result is null
            ? null
            : new GameRoundViewModel
            {
                RoundId = result.RoundId,
                ExternalRoundId = result.ExternalRoundId,
                GameProviderId = result.GameProviderId,
                GameProviderCode = result.GameProviderCode,
                PlayerCode = result.PlayerCode,
                GameId = result.GameId,
                ExternalGameId = result.ExternalGameId,
                CurrencyCode = result.CurrencyCode,
                Status = result.Status,
                GameProviderConfigurationId = result.GameProviderConfigurationId,
                WalletId = result.WalletId,
                CreatedOn = result.CreatedOn,
                ClosedOn = result.ClosedOn,
                UpdatedOn = result.UpdatedOn,
                BrandId = result.BrandId,
                ExternalCreatedOn = result.ExternalCreatedOn,
                ExternalClosedOn = result.ExternalClosedOn,
                BetAmount = result.BetAmount,
                ValidBetAmount = result.ValidBetAmount,
                SettleAmount = result.SettleAmount,
                Commission = result.Commission,
                Turnover = result.Turnover,
                CompanyWinLoss = result.CompanyWinLoss,
                TieBetAdjustment = result.TieBetAdjustment,
                ClientStatus = result.ClientStatus,
                ReopenTurnoverAdjustment = result.ReopenTurnoverAdjustment,
                ReopenedOn = result.ReopenedOn,
                EffectiveBettingPercentage = result.EffectiveBettingPercentage,
                AdditionalData = result.AdditionalData
            };
    }

    /// <summary>
    /// Gets the round detail asynchronous.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="id">The identifier.</param>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Get
    ///         /gamerounds/018919ab-2ece-0000-0001-07e7be4ca7ee/getdetail
    /// </remarks>
    [HttpGet("{id:guid}/getdetail", Name = "RoundDetail_Get")]
    public async ValueTask<GameRoundDetailViewModel?> RoundDetailGetAsync(
        [FromServices] IGameRoundRepository repository,
        Guid id)
    {
        var result = await repository
            .GetRoundDetailAsync(
            id: id,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return result == null
            ? null
            : new GameRoundDetailViewModel
            {
                RoundId = result.RoundId,
                AdditionalData = result.AdditionalData
            };
    }
}
