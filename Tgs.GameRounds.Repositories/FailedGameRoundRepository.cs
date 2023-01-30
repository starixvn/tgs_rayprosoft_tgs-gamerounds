using Microsoft.EntityFrameworkCore;

using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core;
using Tgs.GameRounds.Core.Models;
using Tgs.GameRounds.Repositories.Entities;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRounds.Repositories;

internal class FailedGameRoundRepository : IFailedGameRound
{
    private readonly BetContext _context;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public FailedGameRoundRepository(
        ISerialNumberGenerator serialNumberGenerator,
        BetContext context
        )
    {
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddAsync(
        AddFailedGameRound source,
        CancellationToken cancellationToken = default)
    {
        var id = await _serialNumberGenerator
            .GenerateAsync(
                cancellationToken)
            .ConfigureAwait(false);

        var data = new FailedGameRecordOperation()
        {
            Id = id,
            GameProviderId = source.GameProviderId,
            GameProviderCode = source.GameProviderCode,
            ExternalRoundId = source.ExternalRoundId,
            PlayerCode = source.PlayerCode,
            GameId = source.GameId,
            ExternalId = source.ExternalGameId,
            Detail = source.Detail,
            AutoStatus = (short)source.AutoStatus,
            ProcesStatus = (short)source.ProcesStatus,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        _ = await _context.FailedGameRecordOperations
           .AddAsync(
               data,
               cancellationToken)
           .ConfigureAwait(false);

        _ = await _context
            .SaveChangesAsync(
                cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<FailedGameRoundInfo?> GetAsync(
        Guid gameProviderId,
        string externalRoundId,
        int gameId,
        string playerCode,
        CancellationToken cancellationToken = default)
        => _context.FailedGameRecordOperations
        .Where(x => x.GameProviderId == gameProviderId &&
        x.ExternalRoundId == externalRoundId &&
        x.GameId == gameId &&
        x.PlayerCode == playerCode)
        .Select(x => new FailedGameRoundInfo()
        {
            FailedGameRoundId = x.Id,
            GameProviderId = x.GameProviderId,
            GameProviderCode = x.GameProviderCode,
            ExternalRoundId = x.ExternalRoundId,
            PlayerCode = x.PlayerCode,
            GameId = x.GameId,
            ExternalGameId = x.ExternalId,
            Detail = x.Detail,
            AutoStatus = (FailedGameRoundAutoStatus)x.AutoStatus,
            ProcesStatus = (FailedGameRoundProcesStatus)x.ProcesStatus,
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn,
        })
        .FirstOrDefaultAsync(
            cancellationToken);
}
