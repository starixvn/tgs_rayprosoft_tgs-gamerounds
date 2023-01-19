using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.Core.Models;
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
    /// Adds the round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPost(Name = "AddRound")]
    public ValueTask AddRoundAsync(
        [FromServices] ICommandService<AddRoundCommand> service,
        AddRoundViewModel model)
        => service.ExecuteAsync(new AddRoundCommand(
            ExternalRoundId: model.ExternalRoundId,
            GameProviderId: model.GameProviderId,
            PlayerCode: model.PlayerCode,
            GameId: model.GameId,
            CurrencyCode: model.CurrencyCode,
            Status: model.Status,
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
            PlayerWinLoss: model.PlayerWinLoss,
            ClientStatus: model.ClientStatus,
            AdditionalData: model.AdditionalData));

    /// <summary>
    /// Adds the round batch asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPost("batch", Name = "AddRoundBatch")]
    public async ValueTask<IEnumerable<Guid>> AddRoundBatchAsync(
        [FromServices] IGetService<AddRoundBatchCommand, IEnumerable<Guid>> service,
        AddRoundBatchsViewModel model)
    {
        var addRoundBatches = model.AddRoundBatches.Select(data => new AddRoundBatch(
            ExternalRoundId: data.ExternalRoundId,
            GameProviderId: data.GameProviderId,
            PlayerCode: data.PlayerCode,
            GameId: data.GameId,
            CurrencyCode: data.CurrencyCode,
            Status: data.Status,
            GameProviderConfigurationId: data.GameProviderConfigurationId,
            WalletId: data.WalletId,
            BrandId: data.BrandId,
            ExternalCreatedOn: data.ExternalCreatedOn,
            ExternalClosedOn: data.ExternalClosedOn,
            BetAmount: data.BetAmount,
            ValidBetAmount: data.ValidBetAmount,
            SettleAmount: data.SettleAmount,
            Commission: data.Commission,
            Turnover: data.Turnover,
            PlayerWinLoss: data.PlayerWinLoss,
            ClientStatus: data.ClientStatus,
            AdditionalData: data.AdditionalData)).AsEnumerable();

        return await service.GetAsync(new AddRoundBatchCommand(
               AddRoundBatches: addRoundBatches)).ConfigureAwait(false);
    }

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
            result.EffectiveBettingPercentage,
            result.AdditionalData) : null;
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
            result.EffectiveBettingPercentage,
            result.AdditionalData) : null;
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
            result.EffectiveBettingPercentage,
            result.AdditionalData) : null;
    }

    /// <summary>
    /// Gets the round detail asynchronous.
    /// </summary>
    /// <param name="gameRound">The game round.</param>
    /// <param name="roundid">The roundid.</param>
    /// <returns></returns>
    [HttpGet("{roundid}/getdetail", Name = "GetRoundDetail")]
    public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(
        [FromServices] IGameRound gameRound,
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
        [FromServices] IGameRound gameRound,
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
    /// Gets the status asynchronous.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="roundid">The roundid.</param>
    /// <returns></returns>
    [HttpGet("{roundid}/status", Name = "GameRoundGetStatus")]
    public ValueTask<GameRoundStatus> GetStatusAsync(
        [FromServices] IGameRound repository,
        Guid roundid)
        => repository
            .GetStatusAsync(roundid);

    /// <summary>
    /// Queries the by round identifier asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="gameProviderId">The game provider identifier.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="externalGameId">The external game identifier.</param>
    /// <returns></returns>
    [HttpGet("{gameProviderId}/{externalRoundId}/{externalGameId}/roundids", Name = "GetListByRoundIdQuery")]
    public IAsyncEnumerable<Guid> GetListByRoundIdQueryAsync(
        [FromServices] IQueryService<GetListByRoundIdQuery, Guid> service,
         Guid gameProviderId,
         string externalRoundId,
         string externalGameId)
        => service
            .QueryAsync(new GetListByRoundIdQuery(
                gameProviderId,
                externalRoundId,
                externalGameId));

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
    /// Creates new roundasync.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPost("new", Name = "NewRound")]
    public ValueTask<Guid> NewRoundAsync(
        [FromServices] IGetService<NewRoundQuery, Guid> service,
        NewRoundViewModel model)
        => service.GetAsync(new NewRoundQuery(
            model.ExternalRoundId,
            model.GameProviderId,
            model.PlayerCode,
            model.GameId,
            model.CurrencyCode,
            model.GameProviderConfigurationId,
            model.WalletId,
            model.BrandId,
            model.ExternalCreatedOn));

    /// <summary>
    /// Updates the client status asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/clientstatus", Name = "UpdateClientStatus")]
    public ValueTask UpdateClientStatusAsync(
        [FromServices] ICommandService<UpdateClientStatusCommand> service,
        Guid roundid,
        UpdateClientStatusViewModel model)
        => service
        .ExecuteAsync(new UpdateClientStatusCommand(roundid, model.Status));

    /// <summary>
    /// Updates the round closed asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/closed", Name = "UpdateRoundClosed")]
    public ValueTask UpdateRoundClosedAsync(
        [FromServices] ICommandService<UpdateRoundClosedCommand> service,
        Guid roundid,
        UpdateRoundClosedViewModel model)
        => service.ExecuteAsync(new UpdateRoundClosedCommand(
            RoundId: roundid,
            ExternalClosedOn: model.ExternalClosedOn,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            PlayerWinLoss: model.PlayerWinLoss,
            ClientStatus: model.ClientStatus,
            AdditionalData: model.AdditionalData));

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

    /// <summary>
    /// Updates the round open asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/open", Name = "UpdateRoundOpen")]
    public ValueTask UpdateRoundOpenAsync(
        [FromServices] ICommandService<UpdateRoundOpenCommand> service,
        Guid roundid,
        UpdateRoundOpenViewModel model)
        => service.ExecuteAsync(new UpdateRoundOpenCommand(
            RoundId: roundid,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            Commission: model.Commission,
            Turnover: model.Turnover));

    /// <summary>
    /// Updates the round open asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/open/settle", Name = "UpdateRoundOpenSettle")]
    public ValueTask UpdateRoundOpenSettleAsync(
        [FromServices] ICommandService<UpdateRoundOpenSettleCommand> service,
        Guid roundid,
        UpdateRoundOpenSettleViewModel model)
        => service.ExecuteAsync(new UpdateRoundOpenSettleCommand(
            RoundId: roundid,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            PlayerWinLoss: model.PlayerWinLoss));

    /// <summary>
    /// Updates the round reopen asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/reopen", Name = "UpdateRoundReopen")]
    public ValueTask UpdateRoundReopenAsync(
        [FromServices] ICommandService<UpdateRoundReopenCommand> service,
        Guid roundid,
        UpdateRoundReopenViewModel model)
        => service.ExecuteAsync(new UpdateRoundReopenCommand(
            RoundId: roundid,
            ExternalClosedOn: model.ExternalClosedOn,
            Status: model.Status,
            BetAmount: model.BetAmount,
            ValidBetAmount: model.ValidBetAmount,
            SettleAmount: model.SettleAmount,
            Commission: model.Commission,
            Turnover: model.Turnover,
            PlayerWinLoss: model.PlayerWinLoss,
            ClientStatus: model.ClientStatus,
            AdditionalData: model.AdditionalData));

    /// <summary>
    /// Updates the status asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="roundid">The roundid.</param>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    [HttpPut("{roundid}/status", Name = "UpdateStatus")]
    public ValueTask UpdateStatusAsync(
        [FromServices] ICommandService<UpdateStatusCommand> service,
        Guid roundid,
        UpdateStatusViewModel model)
        => service.ExecuteAsync(new UpdateStatusCommand(
            roundid,
            model.Status,
            model.ExternalClosedOn));
}
