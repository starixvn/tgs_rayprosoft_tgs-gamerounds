using Tgs.GameRounds.Common;
using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core;

public interface IGameRound
{
	ValueTask<Guid?> AddCompanyRoundAsync(
		AddCompanyRound source,
		CancellationToken cancellationToken = default);

	ValueTask<Guid?> AddRoundAsync(
		AddRound source,
		CancellationToken cancellationToken = default);

	IAsyncEnumerable<Guid> AddRoundBatchAsync(
		IEnumerable<AddRoundBatch> sources,
		CancellationToken cancellationToken = default);

	ValueTask AdjustClosedRoundAsync(
		Guid roundId,
		AdjustClosedRound source,
		CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(
		Guid roundId,
		CancellationToken cancellationToken = default);

	ValueTask<GameRoundInfo?> GetAsync(
		Guid roundId,
		CancellationToken cancellationToken = default);

	ValueTask<GameRoundInfo?> GetAsync(
		Guid gameProviderId,
		string externalRoundId,
		int gameId,
		Guid walletId,
		CancellationToken cancellationToken = default);

	ValueTask<GameRoundDetailInfo?> GetRoundDetailAsync(
		Guid roundId,
		CancellationToken cancellationToken = default);

	ValueTask<Guid?> GetRoundIdAsync(
		Guid gameProviderId,
		string externalRoundId,
		int gameId,
		Guid walletId,
		CancellationToken cancellationToken = default);

	ValueTask<GameRoundStatus> GetStatusAsync(
		Guid roundId,
		CancellationToken cancellationToken = default);

	ValueTask ManualCloseRoundAsync(
		Guid roundId,
		ManualCloseRound source,
		CancellationToken cancellationToken = default);

	ValueTask<Guid?> NewRoundAsync(
		NewRound source,
		CancellationToken cancellationToken = default);

	ValueTask UpdateClientStatusAsync(
		Guid roundId,
		GameRoundClientStatus status,
		CancellationToken cancellationToken = default);

	ValueTask UpdateRoundClosedAsync(
		Guid roundId,
		UpdateRoundClosed source,
		CancellationToken cancellationToken = default);

	ValueTask UpdateRoundDetailAsync(
		Guid roundId,
		string additionalData,
		CancellationToken cancellationToken = default);

	ValueTask UpdateRoundOpenAsync(
		Guid roundId,
		UpdateRoundOpen source,
		CancellationToken cancellationToken = default);

	ValueTask UpdateRoundOpenSettleAsync(
		Guid roundId,
		UpdateRoundOpenSettle source,
		CancellationToken cancellationToken = default);

	ValueTask UpdateRoundReopenAsync(
		Guid roundId,
		UpdateRoundReopen source,
		CancellationToken cancellationToken = default);

	ValueTask UpdateStatusAsync(
		Guid roundId,
		GameRoundStatus status,
		DateTimeOffset? externalClosedOn = null,
		CancellationToken cancellationToken = default);
}
