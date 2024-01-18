using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class GameRoundsController : ControllerBase
{
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
    [HttpGet("{id:guid}", Name = "GetGameRound")]
    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(
        [FromServices] IGetService<GetGameRoundQuery, GameRoundResult> service,
        Guid id)
    {
        var query = new GetGameRoundQuery(
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
    /// Gets the game round by external game asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="gameProviderId">The game provider identifier.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="externalGameId">The external game identifier.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <returns></returns>
    [HttpGet("externalgame", Name = "GetGameRoundByExternalGame")]
    public async ValueTask<GameRoundViewModel?> GetGameRoundByExternalGameAsync(
        [FromServices] IGetService<GetGameRoundByExternalGameQuery, GameRoundResult> service,
        Guid gameProviderId,
        string externalRoundId,
        string externalGameId,
        Guid walletId)
    {
        var query = new GetGameRoundByExternalGameQuery(
            GameProviderId: gameProviderId,
            ExternalRoundId: externalRoundId,
            ExternalGameId: externalGameId,
            WalletId: walletId);

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
    /// Gets the game round by game asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="gameProviderId">The game provider identifier.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <returns></returns>
    [HttpGet("game", Name = "GetGameRoundByGame")]
    public async ValueTask<GameRoundViewModel?> GetGameRoundByGameAsync(
        [FromServices] IGetService<GetGameRoundByGameQuery, GameRoundResult> service,
        [FromQuery] Guid gameProviderId,
        [FromQuery] string externalRoundId,
        [FromQuery] int gameId,
        [FromQuery] Guid walletId)
    {
        var query = new GetGameRoundByGameQuery(
            GameProviderId: gameProviderId,
            ExternalRoundId: externalRoundId,
            GameId: gameId,
            WalletId: walletId);

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
    [HttpGet("{id:guid}/getdetail", Name = "GetRoundDetail")]
    public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(
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
