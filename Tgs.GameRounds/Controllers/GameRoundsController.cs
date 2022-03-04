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
        [HttpGet("{rounid}/status", Name = "GameRoundsStatus")]
        public async ValueTask<GameRoundStatus?> GetGameRoundStatusAsync(
                   [FromServices] IGameRoundRepository repository,
                   Guid rounid)
        {
            var round = await repository.GetAsync(rounid).ConfigureAwait(false);
            return round != null ? round.Status : null;
        }

        [HttpPost("new", Name = "GameRoundsNewRound")]
        public async ValueTask<NewRoundResultViewModel> NewRoundAsync(
            [FromServices] IGetService<NewRoundQuery, NewRoundResult> service,
            [FromBody] NewRoundQueryViewModel model)
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

        [HttpPost("update", Name = "GameRoundsUpdate")]
        public async ValueTask NewRoundAsync(
            [FromServices] ICommandService<UpdateRoundCommand> service,
            [FromBody] UpdateRoundCommandViewModel model)
            => await service.ExecuteAsync(new(
                model.RoundId,
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
    }
}
