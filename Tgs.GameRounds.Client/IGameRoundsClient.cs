using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.Client;

public interface IGameRoundsClient
{
	ValueTask<GameRoundResponse> AddCompanyRoundAsync(AddCompanyRoundViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> AddRoundAsync(AddRoundViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundBatchResponse> AddRoundBatchAsync(AddRoundBatchsViewModel model, CancellationToken cancellationToken = default);

	ValueTask<GameRoundResponse> AdjustClosedRoundAsync(Guid roundId, AdjustClosedRoundViewModel model, CancellationToken cancellationToken = default);

	ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, string externalGameId, Guid walletId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundDataResponse> GetGameRoundAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Guid> GetListByRoundIdAsync(Guid gameProviderId, string externalRoundId, string externalGameId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundDetailDataResponse> GetRoundDetailAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundIdResponse> GetRoundIdAsync(Guid gameProviderId, string externalRoundId, int gameId, Guid walletId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundRoundStatusResponse> GetStatusAsync(Guid roundId, CancellationToken cancellationToken = default);

	ValueTask<GameRoundResponse> ManualCloseRoundAsync(Guid roundId, ManualCloseRoundViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundRoundIdResponse> NewRoundAsync(NewRoundViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateClientStatusAsync(Guid roundId, GameRoundClientStatus status, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateRoundClosedAsync(Guid roundId, UpdateRoundClosedViewModel model, CancellationToken cancellationToken = default);

	ValueTask<GameRoundResponse> UpdateRoundDetailAsync(Guid roundId, string additionalData, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateRoundOpenAsync(Guid roundId, UpdateRoundOpenViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateRoundOpenSettleAsync(Guid roundId, UpdateRoundOpenSettleViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateRoundReopenAsync(Guid roundId, UpdateRoundReopenViewModel model, CancellationToken cancellationToken = default);

	[Obsolete("Please use IGameRoundsWorkClient.ExecuteAsync instead", false)]
	ValueTask<GameRoundResponse> UpdateStatusAsync(Guid roundId, GameRoundStatus status, DateTimeOffset? externalClosedOn = null, CancellationToken cancellationToken = default);
}
