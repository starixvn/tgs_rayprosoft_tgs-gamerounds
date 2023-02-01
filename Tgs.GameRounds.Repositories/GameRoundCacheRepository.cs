using System.Text.Json;

using StackExchange.Redis;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Repositories;

internal class GameRoundCacheRepository : IGameRoundRepository
{
    private const string KeyExist = "gameround:exist:{0}";
    private const string KeyRoundId = "gameround:roundid:{0}:{1}:{2}:{3}";
    private readonly IDatabase _database;
    private readonly TimeSpan _expiry;
    private readonly IGameRoundRepository _gameRound;

    public GameRoundCacheRepository(
        IGameRoundRepository gameRound,
        IDatabase database,
        TimeSpan expiry)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _gameRound = gameRound ?? throw new ArgumentNullException(nameof(gameRound));
        _expiry = expiry;
    }

    public ValueTask<Guid?> AddCompanyRoundAsync(
        AddCompanyRound source,
        CancellationToken cancellationToken = default)
        => _gameRound.AddCompanyRoundAsync(
            source,
            cancellationToken);

    public ValueTask<Guid?> AddRoundAsync(
        AddRound source,
        CancellationToken cancellationToken = default)
        => _gameRound.AddRoundAsync(
            source,
            cancellationToken);

    public ValueTask AdjustClosedRoundAsync(
                Guid roundId,
        AdjustClosedRound source,
        CancellationToken cancellationToken = default)
        => _gameRound.AdjustClosedRoundAsync(
            roundId: roundId,
            source: source,
            cancellationToken: cancellationToken);

    public async ValueTask<bool> ExistAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
    {
        string key = string.Format(KeyExist, roundId).ToLower();

        var exist = await _database.KeyExistsAsync(key).ConfigureAwait(false);
        if (!exist)
        {
            exist = await _gameRound.ExistAsync(
               roundId: roundId,
               cancellationToken: cancellationToken).ConfigureAwait(false);

            if (exist)
            {
                var value = JsonSerializer.Serialize(new
                {
                    roundId,
                    LogOn = DateTimeOffset.UtcNow.ToString("O")
                });

                _ = await _database.StringSetAsync(
                    key: key,
                    value: value,
                    expiry: _expiry,
                    when: When.Always,
                    flags: CommandFlags.None).ConfigureAwait(false);
            }
        }

        return exist;
    }

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => _gameRound.GetAsync(
            roundId,
            cancellationToken: cancellationToken);

    public ValueTask<GameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
        => _gameRound.GetAsync(
            gameProviderId: gameProviderId,
            externalRoundId: externalRoundId,
            gameId: gameId,
            walletId: walletId,
            cancellationToken: cancellationToken);

    public ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
            Guid roundId,
        CancellationToken cancellationToken = default)
        => _gameRound.GetRoundDetailAsync(
            roundId: roundId,
            cancellationToken: cancellationToken);

    public async ValueTask<Guid?> GetRoundIdAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        Guid walletId,
        CancellationToken cancellationToken = default)
    {
        string key = string.Format(KeyRoundId, gameProviderId, externalRoundId, gameId, walletId).ToLower();

        if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
        {
            var roundId = await _gameRound.GetRoundIdAsync(
                                gameProviderId: gameProviderId,
                                externalRoundId: externalRoundId,
                                gameId: gameId,
                                walletId: walletId,
                                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (!roundId.HasValue)
            {
                return roundId;
            }

            _ = await _database.StringSetAsync(
                key: key,
                value: roundId.Value.ToString(),
                expiry: _expiry,
                when: When.Always,
                flags: CommandFlags.None).ConfigureAwait(false);

            return roundId;
        }

        return Guid.TryParse(await _database.StringGetAsync(key).ConfigureAwait(false), out var tempRoundId)
            ? tempRoundId
            : null;
    }

    public ValueTask<GameRoundStatus> GetStatusAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
        => _gameRound.GetStatusAsync(
            roundId: roundId,
            cancellationToken: cancellationToken);

    public ValueTask ManualCloseRoundAsync(
        Guid roundId,
        ManualCloseRound source,
        CancellationToken cancellationToken = default)
        => _gameRound.ManualCloseRoundAsync(
            roundId: roundId,
            source: source,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundClosedAsync(
        Guid roundId,
        UpdateRoundClosed source,
        CancellationToken cancellationToken = default)
        => _gameRound.UpdateRoundClosedAsync(
            roundId: roundId,
            source: source,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundDetailAsync(
        Guid roundId,
        string additionalData,
        CancellationToken cancellationToken = default)
        => _gameRound.UpdateRoundDetailAsync(
            roundId: roundId,
            additionalData: additionalData,
            cancellationToken: cancellationToken);

    public ValueTask UpdateRoundOpenAsync(
        Guid roundId,
        UpdateRoundOpen source,
        CancellationToken cancellationToken = default)
         => _gameRound.UpdateRoundOpenAsync(
             roundId: roundId,
             source: source,
             cancellationToken: cancellationToken);

    public ValueTask UpdateRoundReopenAsync(
        Guid roundId,
        UpdateRoundReopen source,
        CancellationToken cancellationToken = default)
        => _gameRound.UpdateRoundReopenAsync(
            roundId: roundId,
            source: source,
            cancellationToken: cancellationToken);
}
