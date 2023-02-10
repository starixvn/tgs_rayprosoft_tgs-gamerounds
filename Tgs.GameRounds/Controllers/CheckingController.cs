using Microsoft.AspNetCore.Mvc;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;
using Tgs.GameRounds.Core.HandleServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Controllers;

[Route("[controller]")]
[ApiController]
public class CheckingController : ControllerBase
{
    [HttpPost(Name = "CheckingStatus")]
    public ValueTask<GameRoundCheckingStatus> GetCheckingStatusAsync(
        [FromServices] IGetService<GameRoundCheckingQuery, GameRoundCheckingStatus> service,
        GameRoundCheckingViewModel source)
        => service
        .GetAsync(
            query: new GameRoundCheckingQuery(
                source.ExternalRoundId,
                source.GameProviderId,
                source.GameId,
                source.PlayerCode,
                source.WalletId,
                source.Status,
                source.ClientStatus,
                source.ExternalCreatedOn,
                source.BetAmount,
                source.ValidBetAmount,
                source.SettleAmount,
                source.Commission,
                source.Turnover,
                source.PlayerWinLoss),
            cancellationToken: HttpContext.RequestAborted);
}
