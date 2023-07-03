using Tgs.GameRounds.Core.Exceptions;
using Tgs.ServiceUtility;

namespace Tgs.GameRounds.Core.ApplicationServices;

internal class UpdateRoundDetailCommandService : ICommandService<UpdateRoundDetailCommand>
{
    private readonly IGameRoundRepository _repository;

    public UpdateRoundDetailCommandService(IGameRoundRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async ValueTask ExecuteAsync(
        UpdateRoundDetailCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await _repository
            .ExistAsync(
            id: command.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false))
        {
            throw new GameRoundDoesNotExistException(command.RoundId);
        }

        await _repository
             .UpdateRoundDetailAsync(
            id: command.RoundId,
            additionalData: command.AdditionalData,
            cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }
}
