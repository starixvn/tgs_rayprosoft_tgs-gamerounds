using System.ComponentModel.DataAnnotations;
using Tgs.GameRounds.Common.ViewModels;

namespace Tgs.GameRounds.ViewModels;

public record GameRoundByGameViewModel
{
    [Required]
    public Guid GameProviderId { get; init; }
    [Required]
    public string ExternalRoundId { get; init; } = default!;
    [Required]
    public int GameId { get; init; }
    [Required]
    public Guid WalletId { get; init; }
}
