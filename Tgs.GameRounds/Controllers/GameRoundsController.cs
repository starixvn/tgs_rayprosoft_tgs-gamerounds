using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameRoundsController : ControllerBase
    {
        [HttpGet("{rounid}/status", Name = "GameRoundGetStatus")]
        public async ValueTask<GameRoundStatus?> GameRoundGetStatusAsync(
                   [FromServices] IGameRoundRepository repository,
                   Guid rounid)
        {
            var round = await repository.GetAsync(rounid).ConfigureAwait(false);
            return round != null ? round.Status : null;
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
        [HttpPost("new", Name = "GameRoundsNewRound")]
        public async ValueTask<NewRoundResultViewModel> GameRoundsNewRoundAsync(
                [FromServices] IGetService<NewRoundQuery, NewRoundResult> service,
                NewRoundQueryViewModel model)
        {
            var result = await service.GetAsync(new(
                        model.ExternalRoundId,
                        model.GameProviderId,
                        model.PlayerCode,
                        model.GameId,
                        model.CurrencyCode,
                        model.GameProviderConfigurationId,
                        model.WalletId,
                        model.BrandId,
                        model.ExternalCreatedOn)).ConfigureAwait(false);
            return new(
                result.Success,
                result.RoundId);
        }

        /// <summary>
        /// Games the rounds update round asynchronous.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="roundid">The roundid.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Example</b><br />
        ///
        ///     Put /6208f0ac-7f01-0000-0001-0000310c07fd
        ///         {
        ///             "status": 1,
        ///             "closedOn": null,
        ///             "externalClosedOn": null,
        ///             "betAmount": 10,
        ///             "validBetAmount": 10,
        ///             "settleAmount": null,
        ///             "commission": null,
        ///             "turnover": null,
        ///             "winLoss":null,
        ///             "tieBetAdjustment": null,
        ///             "clientStatus": 1,
        ///             "reopenTurnoverAdjustment": null,
        ///             "reopenedOn": null,
        ///             "additionalData": null
        ///         }
        /// </remarks>
        [HttpPut("{roundid}", Name = "GameRoundsUpdateRound")]
        public async ValueTask<UpdateRoundResultViewModel> GameRoundsUpdateRoundAsync(
                [FromServices] IGetService<UpdateRoundQuery, UpdateRoundResult> service,
                Guid roundid,
                UpdateRoundQueryViewModel model)
        {
            if (!Enum.IsDefined(model.ClientStatus))
            {
                return new(false);
            }

            if (!Enum.IsDefined(model.Status))
            {
                return new(false);
            }

            var result = await service.GetAsync(new(
                roundid,
                model.Status,
                model.ClosedOn,
                model.ExternalClosedOn,
                model.BetAmount,
                model.ValidBetAmount,
                model.SettleAmount,
                model.Commission,
                model.Turnover,
                model.WinLoss,
                model.TieBetAdjustment,
                model.ClientStatus,
                model.ReopenTurnoverAdjustment,
                model.ReopenedOn,
                model.AdditionalData)).ConfigureAwait(false);
            return new(result.Success);
        }
    }
}
