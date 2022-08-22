using Tgs.GameRounds.Common;

namespace Tgs.GameRounds.Core;

public record FailedGameRoundInfo
{
	public Guid FailedGameRoundId { get; set; }

	public Guid GameProviderId { get; set; }

	public string? GameProviderCode { get; set; }

	public string ExternalRoundId { get; set; } = default!;

	public string PlayerCode { get; set; } = default!;

	public int GameId { get; set; }

	public string ExternalGameId { get; set; } = default!;

	public string Detail { get; set; } = default!;

	public FailedGameRoundAutoStatus AutoStatus { get; set; }

	public FailedGameRoundProcesStatus ProcesStatus { get; set; }

	public DateTimeOffset CreatedOn { get; set; }

	public DateTimeOffset UpdatedOn { get; set; }
}
