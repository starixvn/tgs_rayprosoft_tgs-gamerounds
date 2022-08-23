using Tgs.GameRounds.Core.Models;

namespace Tgs.GameRounds.Core.ApplicationServices;

public record AddRoundBatchCommand(IEnumerable<AddRoundBatch> AddRoundBatches);
