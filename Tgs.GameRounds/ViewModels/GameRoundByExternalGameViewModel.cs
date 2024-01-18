using System.ComponentModel.DataAnnotations;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.ViewModels;

public record GameRoundByExternalGameViewModel
{
    [Required]
    public Guid GameProviderId { get; init; }
    [Required]
    public string ExternalRoundId { get; init; } = default!;
    [Required]
    public string ExternalGameId { get; init; } = default!;
    [Required]
    public Guid WalletId { get; init; }
}
