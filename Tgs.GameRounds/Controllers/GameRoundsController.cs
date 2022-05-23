using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.ApplicationServices;
using Tgs.GameRounds.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameRoundsController : ControllerBase
    {
        /// <summary>
        /// Adds the round asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost(Name = "Add")]
        public async ValueTask AddRoundAsync(
            [FromServices] ICommandService<AddRoundCommand> service,
            AddRoundViewModel model)
            => await service.ExecuteAsync(new AddRoundCommand(
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
                AdditionalData: model.AdditionalData)).ConfigureAwait(false);

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
            Guid gameProviderId,
            string externalRoundId,
            int gameId,
            Guid walletId)
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
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <returns></returns>
        [HttpGet("{roundid}/getdetail", Name = "GetRoundDetail")]
        public async ValueTask<GameRoundDetailViewModel?> GetRoundDetailAsync(
                    [FromServices] IGetService<GetRoundDetailQuery, GameRoundDetailResult> service,
                    Guid roundid)
        {
            var result = await service
                .GetAsync(new(roundid))
                .ConfigureAwait(false);

            return result != null ? new GameRoundDetailViewModel(
                result.RoundId,
                result.AdditionalData) : null;
        }

        /// <summary>
        /// Gets the status asynchronous.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="roundid">The roundid.</param>
        /// <returns></returns>
        [HttpGet("{roundid}/status", Name = "GameRoundGetStatus")]
        public async ValueTask<GameRoundStatus?> GetStatusAsync(
            [FromServices] IGameRound repository,
            Guid roundid)
        {
            var round = await repository.GetAsync(roundid).ConfigureAwait(false);
            return round?.Status;
        }

        /// <summary>
        /// Creates new roundasync.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Post
        ///         {
        ///             "externalRoundId": "A22C70E6-61DD-7692-1B1C-A7488034EE71",
        ///             "gameProviderId": "a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5",
        ///             "playerCode": "4ucmqplxb029",
        ///             "gameId": 1000001,
        ///             "currencyCode": "CNY",
        ///             "gameProviderConfigurationId": "C6A14612-C856-4CF7-3605-9BBE531F847D",
        ///             "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///             "brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
        ///             "externalCreatedOn": "2022-03-07T01:37:01.625Z"
        ///         }
        /// </remarks>
        [HttpPost("new", Name = "NewRound")]
        public ValueTask<Guid> NewRoundAsync(
            [FromServices] IGetService<NewRoundQuery, Guid> service,
            NewRoundQueryViewModel model)
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
        public async ValueTask UpdateClientStatusAsync(
            [FromServices] ICommandService<UpdateClientStatusCommand> service,
            Guid roundid,
            UpdateClientStatusViewModel model)
            => await service.ExecuteAsync(new UpdateClientStatusCommand(
                roundid,
                model.Status)).ConfigureAwait(false);

        /// <summary>
        /// Updates the round closed asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/closed", Name = "UpdateRoundClosed")]
        public async ValueTask UpdateRoundClosedAsync(
            [FromServices] ICommandService<UpdateRoundClosedCommand> service,
            Guid roundid,
            UpdateRoundClosedViewModel model)
            => await service.ExecuteAsync(new UpdateRoundClosedCommand(
                RoundId: roundid,
                ExternalClosedOn: model.ExternalClosedOn,
                BetAmount: model.BetAmount,
                ValidBetAmount: model.ValidBetAmount,
                SettleAmount: model.SettleAmount,
                Commission: model.Commission,
                Turnover: model.Turnover,
                PlayerWinLoss: model.PlayerWinLoss,
                ClientStatus: model.ClientStatus,
                AdditionalData: model.AdditionalData)).ConfigureAwait(false);

        /// <summary>
        /// Updates the round detail asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/detail", Name = "UpdateRoundDetail")]
        public async ValueTask UpdateRoundDetailAsync(
            [FromServices] ICommandService<UpdateRoundDetailCommand> service,
            Guid roundid,
            UpdateRoundDetailViewModel model)
            => await service.ExecuteAsync(new(
                roundid,
                model.AdditionalData)).ConfigureAwait(false);

        /// <summary>
        /// Updates the round open asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/open", Name = "UpdateRoundOpen")]
        public async ValueTask UpdateRoundOpenAsync(
            [FromServices] ICommandService<UpdateRoundOpenCommand> service,
            Guid roundid,
            UpdateRoundOpenViewModel model)
            => await service.ExecuteAsync(new UpdateRoundOpenCommand(
                RoundId: roundid,
                BetAmount: model.BetAmount,
                ValidBetAmount: model.ValidBetAmount,
                Commission: model.Commission,
                Turnover: model.Turnover)).ConfigureAwait(false);

        /// <summary>
        /// Updates the round open asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/open/settle", Name = "UpdateRoundOpenSettle")]
        public async ValueTask UpdateRoundOpenSettleAsync(
            [FromServices] ICommandService<UpdateRoundOpenSettleCommand> service,
            Guid roundid,
            UpdateRoundOpenSettleViewModel model)
            => await service.ExecuteAsync(new UpdateRoundOpenSettleCommand(
                RoundId: roundid,
                BetAmount: model.BetAmount,
                ValidBetAmount: model.ValidBetAmount,
                SettleAmount: model.SettleAmount,
                Commission: model.Commission,
                Turnover: model.Turnover,
                PlayerWinLoss: model.PlayerWinLoss)).ConfigureAwait(false);

        /// <summary>
        /// Updates the round reopen asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/reopen", Name = "UpdateRoundReopen")]
        public async ValueTask UpdateRoundReopenAsync(
            [FromServices] ICommandService<UpdateRoundReopenCommand> service,
            Guid roundid,
            UpdateRoundReopenViewModel model)
            => await service.ExecuteAsync(new UpdateRoundReopenCommand(
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
                AdditionalData: model.AdditionalData)).ConfigureAwait(false);

        /// <summary>
        /// Updates the status asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut("{roundid}/status", Name = "UpdateStatus")]
        public async ValueTask UpdateStatusAsync(
            [FromServices] ICommandService<UpdateStatusCommand> service,
            Guid roundid,
            UpdateStatusViewModel model)
            => await service.ExecuteAsync(new UpdateStatusCommand(
                roundid,
                model.Status,
                model.ExternalClosedOn)).ConfigureAwait(false);
    }
}
