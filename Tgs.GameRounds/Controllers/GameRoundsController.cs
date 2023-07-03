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
    /// Adds the company round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The model.</param>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///     
    ///         {
    ///             "externalRoundId": "50-1687319019-331826368-2",
    ///             "gameProviderId": "a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5",
    ///             "playerCode": "4ucmqplxb029",
    ///             "gameId": 1000017,
    ///             "currencyCode": "CNY",
    ///             "gameProviderConfigurationId": "fa0d7706-f910-4e98-bf71-bd7084140304",
    ///             "walletId": "98f8b1f9-897e-443f-36d3-08da2e3e62fb",
    ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
    ///             "externalCreatedOn": "2023-06-21T11:43:39.000+08:00",
    ///             "externalClosedOn": "2023-06-21T11:44:03.000+08:00",
    ///             "betAmount": 4,
    ///             "validBetAmount": 4,
    ///             "settleAmount": 0,
    ///             "commission": 0,
    ///             "turnover": 4,
    ///             "companyWinLoss": 4,
    ///             "clientStatus": "Loss",
    ///             "operatorId": "7349b697-96e4-ed11-a95d-8c554ae8ab6f",
    ///             "remark": "測試"
    ///         }
    /// </remarks>
    [HttpPost("company", Name = "AddCompanyRound")]
    public ValueTask AddCompanyRoundAsync(
        [FromServices] ICommandService<AddCompanyRoundCommand> service,
        AddCompanyRoundViewModel source)
    {
        var command = new AddCompanyRoundCommand(
            ExternalRoundId: source.ExternalRoundId,
            GameProviderId: source.GameProviderId,
            PlayerCode: source.PlayerCode,
            GameId: source.GameId,
            CurrencyCode: source.CurrencyCode,
            GameProviderConfigurationId: source.GameProviderConfigurationId,
            WalletId: source.WalletId,
            BrandId: source.BrandId,
            ExternalCreatedOn: source.ExternalCreatedOn,
            ExternalClosedOn: source.ExternalClosedOn,
            BetAmount: source.BetAmount,
            ValidBetAmount: source.ValidBetAmount,
            SettleAmount: source.SettleAmount,
            Commission: source.Commission,
            Turnover: source.Turnover,
            CompanyWinLoss: source.CompanyWinLoss,
            ClientStatus: source.ClientStatus,
            OperatorId: source.OperatorId,
            Remark: source.Remark);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Adjusts the closed round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="source">The model.</param>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Put 
    ///         01891a83-9c29-0000-0001-07e769564c64/adjust/closed
    ///     
    ///         {
    ///             "externalClosedOn": "2023-06-21T11:44:03.000+08:00",
    ///             "betAmount": 4,
    ///             "validBetAmount": 4,
    ///             "settleAmount": 8,
    ///             "commission": 0,
    ///             "turnover": 4,
    ///             "companyWinLoss": -4,
    ///             "clientStatus": "Win",
    ///             "operatorId": "7349b697-96e4-ed11-a95d-8c554ae8ab6f",
    ///             "remark": "test"
    ///         }
    /// </remarks>
    [HttpPut("{id:guid}/adjust/closed", Name = "AdjustClosedRound")]
    public ValueTask AdjustClosedRoundAsync(
        [FromServices] ICommandService<AdjustClosedRoundCommand> service,
        Guid id,
        AdjustClosedRoundViewModel source)
    {
        var command = new AdjustClosedRoundCommand(
            RoundId: id,
            ExternalClosedOn: source.ExternalClosedOn,
            BetAmount: source.BetAmount,
            ValidBetAmount: source.ValidBetAmount,
            SettleAmount: source.SettleAmount,
            Commission: source.Commission,
            Turnover: source.Turnover,
            CompanyWinLoss: source.CompanyWinLoss,
            ClientStatus: source.ClientStatus,
            OperatorId: source.OperatorId,
            Remark: source.Remark);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
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

    /// <summary>
    /// Manuals the close round asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    [HttpPut("{id:guid}/manual/closed", Name = "ManualCloseRound")]
    public ValueTask ManualCloseRoundAsync(
        [FromServices] ICommandService<ManualCloseRoundCommand> service,
        Guid id,
        ManualCloseRoundViewModel source)
    {
        var command = new ManualCloseRoundCommand(
            RoundId: id,
            ExternalClosedOn: source.ExternalClosedOn,
            BetAmount: source.BetAmount,
            ValidBetAmount: source.ValidBetAmount,
            SettleAmount: source.SettleAmount,
            Commission: source.Commission,
            Turnover: source.Turnover,
            CompanyWinLoss: source.CompanyWinLoss,
            ClientStatus: source.ClientStatus,
            OperatorId: source.OperatorId,
            Remark: source.Remark);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }
}
