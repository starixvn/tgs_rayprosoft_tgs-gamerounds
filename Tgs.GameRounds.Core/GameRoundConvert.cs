using Tgs.GameCurrency.Client;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

internal class GameRoundConvert : IGameRoundConvert
{
    private readonly IGameCurrencyClient _client;
    private readonly IGameProvider _gameProvider;

    public GameRoundConvert(
        IGameProvider gameProvider,
        IGameCurrencyClient client)
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameRoundConvertResult> ConvertPlatformRoundsAsync(GameRoundConvertRequest request, CancellationToken cancellationToken = default)
    {
        var gameProviderCode = await _gameProvider.GetGameProviderCodeAsync(
            gameProviderId: request.GameProviderId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var convertPlatformRoundsRequest = new ConvertPlatformRoundsRequest(
            BetAmount: request.BetAmount,
            ValidBetAmount: request.ValidBetAmount,
            SettleAmount: request.SettleAmount,
            Commission: request.Commission,
            Turnover: request.Turnover,
            WinLoss: request.PlayerWinLoss);

        var result = await _client.ConvertPlatformRoundsAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: request.CurrencyCode,
            request: convertPlatformRoundsRequest,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new GameRoundConvertResult(
            BetAmount: result.BetAmount,
            Commission: result.Commission,
            SettleAmount: result.SettleAmount,
            Turnover: result.Turnover,
            ValidBetAmount: result.ValidBetAmount,
            PlayerWinLoss: result.WinLoss);
    }
}
