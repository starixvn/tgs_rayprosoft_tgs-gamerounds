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
    /// Adds the company round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPost("company", Name = "AddCompanyRound")]
    public ValueTask AddCompanyRoundAsync(
        [FromServices] ICommandService<AddCompanyRoundCommand> service,
        AddCompanyRoundViewModel model)
        => service.ExecuteAsync(new AddCompanyRoundCommand(
            ExternalRoundId: model.ExternalRoundId,
            GameProviderId: model.GameProviderId,
            PlayerCode: model.PlayerCode,
            GameId: model.GameId,
            CurrencyCode: model.CurrencyCode,
            GameProviderConfigurationId: model.GameProviderConfigurationId,
            WalletId: model.WalletId,
            BrandId: model.BrandId,
            ExternalCreatedOn: model.ExternalCreatedOn,
            ExternalClosedOn: model.ExternalClosedOn,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            CompanyWinLoss: model.CompanyWinLoss,
            ClientStatus: model.ClientStatus,
            OperatorId: model.OperatorId,
            Remark: model.Remark));

    /// <summary>
    /// Adjusts the closed round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/adjust/closed", Name = "AdjustClosedRound")]
    public ValueTask AdjustClosedRoundAsync(
        [FromServices] ICommandService<AdjustClosedRoundCommand> service,
        Guid roundid,
        AdjustClosedRoundViewModel model)
        => service.ExecuteAsync(new AdjustClosedRoundCommand(
            RoundId: roundid,
            ExternalClosedOn: model.ExternalClosedOn,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            CompanyWinLoss: model.CompanyWinLoss,
            ClientStatus: model.ClientStatus,
            OperatorId: model.OperatorId,
            Remark: model.Remark));

    /// <summary>
    /// Gets the game round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <returns></returns>
    [HttpGet("{roundid}", Name = "GetGameRound")]
    public async ValueTask<GameRoundViewModel?> GetGameRoundAsync(
        [FromServices] IGetService<GetGameRoundQuery, GameRoundResult> service,
        Guid roundid)
    {
        var result = await service.GetAsync(new(roundid)).ConfigureAwait(false);

        return result != null ? new GameRoundViewModel(
            result.RoundId,
            result.ExternalRoundId,
            result.GameProviderId,
            result.GameProviderCode,
            result.PlayerCode,
            result.GameId,
            result.ExternalGameId,
            result.CurrencyCode,
            result.Status,
            result.GameProviderConfigurationId,
            result.WalletId,
            result.CreatedOn,
            result.ClosedOn,
            result.UpdatedOn,
            result.BrandId,
            result.ExternalCreatedOn,
            result.ExternalClosedOn,
            result.BetAmount,
            result.ValidBetAmount,
            result.SettleAmount,
            result.Commission,
            result.Turnover,
            result.CompanyWinLoss,
            result.TieBetAdjustment,
            result.ClientStatus,
            result.ReopenTurnoverAdjustment,
            result.ReopenedOn,
            result.EffectiveBettingPercentage) : null;
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
        var result = await service.GetAsync(new(
            gameProviderId,
            externalRoundId,
            externalGameId,
            walletId)).ConfigureAwait(false);

        return result != null ? new GameRoundViewModel(
            result.RoundId,
            result.ExternalRoundId,
            result.GameProviderId,
            result.GameProviderCode,
            result.PlayerCode,
            result.GameId,
            result.ExternalGameId,
            result.CurrencyCode,
            result.Status,
            result.GameProviderConfigurationId,
            result.WalletId,
            result.CreatedOn,
            result.ClosedOn,
            result.UpdatedOn,
            result.BrandId,
            result.ExternalCreatedOn,
            result.ExternalClosedOn,
            result.BetAmount,
            result.ValidBetAmount,
            result.SettleAmount,
            result.Commission,
            result.Turnover,
            result.CompanyWinLoss,
            result.TieBetAdjustment,
            result.ClientStatus,
            result.ReopenTurnoverAdjustment,
            result.ReopenedOn,
            result.EffectiveBettingPercentage) : null;
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
        var result = await service.GetAsync(new(
            gameProviderId,
            externalRoundId,
            gameId,
            walletId)).ConfigureAwait(false);

        return result != null ? new GameRoundViewModel(
            result.RoundId,
            result.ExternalRoundId,
            result.GameProviderId,
            result.GameProviderCode,
            result.PlayerCode,
            result.GameId,
            result.ExternalGameId,
            result.CurrencyCode,
            result.Status,
            result.GameProviderConfigurationId,
            result.WalletId,
            result.CreatedOn,
            result.ClosedOn,
            result.UpdatedOn,
            result.BrandId,
            result.ExternalCreatedOn,
            result.ExternalClosedOn,
            result.BetAmount,
            result.ValidBetAmount,
            result.SettleAmount,
            result.Commission,
            result.Turnover,
            result.CompanyWinLoss,
            result.TieBetAdjustment,
            result.ClientStatus,
            result.ReopenTurnoverAdjustment,
            result.ReopenedOn,
            result.EffectiveBettingPercentage) : null;
    }

    /// <summary>
    /// Gets the round detail asynchronous.
    /// </summary>
    /// <param name="gameRound">The game round.</param>
    /// <param name="roundid">The roundid.</param>
    /// <returns></returns>
    [HttpGet("{roundid}/getdetail", Name = "GetRoundDetail")]
    public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(
        [FromServices] IGameRoundRepository gameRound,
        Guid roundid)
    {
        var result = await gameRound
            .GetRoundDetailAsync(roundid)
            .ConfigureAwait(false);

        return result != null ? new GameRoundDetailViewModel(
            result.RoundId,
            result.AdditionalData) : null;
    }

    /// <summary>
    /// Gets the round identifier asynchronous.
    /// </summary>
    /// <param name="gameRound">The game round.</param>
    /// <param name="gameProviderId">The game provider identifier.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="walletId">The wallet identifier.</param>
    /// <returns></returns>
    [HttpGet("getroundid", Name = "GetRoundId")]
    public ValueTask<Guid?> GetRoundIdAsync(
        [FromServices] IGameRoundRepository gameRound,
        [FromQuery] Guid gameProviderId,
        [FromQuery] string externalRoundId,
        [FromQuery] int gameId,
        [FromQuery] Guid walletId)
        => gameRound.GetRoundIdAsync(
            gameProviderId,
            externalRoundId,
            gameId,
            walletId);

    /// <summary>
    /// Manuals the close round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/manual/closed", Name = "ManualCloseRound")]
    public ValueTask ManualCloseRoundAsync(
        [FromServices] ICommandService<ManualCloseRoundCommand> service,
        Guid roundid,
        ManualCloseRoundViewModel model)
        => service.ExecuteAsync(new ManualCloseRoundCommand(
            RoundId: roundid,
            ExternalClosedOn: model.ExternalClosedOn,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            CompanyWinLoss: model.CompanyWinLoss,
            ClientStatus: model.ClientStatus,
            OperatorId: model.OperatorId,
            Remark: model.Remark));

    /// <summary>
    /// Updates the round detail asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/detail", Name = "UpdateRoundDetail")]
    public ValueTask UpdateRoundDetailAsync(
        [FromServices] ICommandService<UpdateRoundDetailCommand> service,
        Guid roundid,
        UpdateRoundDetailViewModel model)
        => service.ExecuteAsync(new UpdateRoundDetailCommand(roundid, model.AdditionalData));
}
