using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tgs.GameRounds.Repositories
{
    public record GameRoundRecord
    {
        /// <summary>
        /// The game round record identifier
        /// </summary>
        [BsonId]
        public Guid GameRoundRecordId { get; set; }

        /// <summary>
        /// The round identifier
        /// </summary>
        [BsonElement("round_id")]
        public Guid RoundId { get; set; }

        /// <summary>
        /// The game provider identifier
        /// </summary>
        [BsonElement("game_provider_id")]
        public Guid GameProviderId { get; set; }

        /// <summary>
        /// The game provider configuration identifier
        /// </summary>
        [BsonElement("game_provider_configuration_id")]
        public Guid GameProviderConfigurationId { get; set; }

        /// <summary>
        /// The external round identifier
        /// </summary>
        [BsonElement("external_round_id")]
        [BsonRepresentation(BsonType.String)]
        public string ExternalRoundId { get; set; } = default!;

        /// <summary>
        /// The brand identifier
        /// </summary>
        [BsonElement("brand_id")]
        public Guid BrandId { get; set; }

        /// <summary>
        /// The player code
        /// </summary>
        [BsonElement("player_code")]
        [BsonRepresentation(BsonType.String)]
        public string PlayerCode { get; set; } = default!;

        /// <summary>
        /// The wallet identifier
        /// </summary>
        [BsonElement("wallet_id")]
        public Guid WalletId { get; set; }

        /// <summary>
        /// The game identifier
        /// </summary>
        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.Int32)]
        public int GameId { get; set; }

        /// <summary>
        /// The currency code
        /// </summary>
        [BsonElement("currency_code")]
        [BsonRepresentation(BsonType.String)]
        public string CurrencyCode { get; set; } = default!;

        /// <summary>
        /// The status
        /// </summary>
        [BsonElement("status")]
        [BsonRepresentation(BsonType.Int32)]
        public int Status { get; set; }

        /// <summary>
        /// The created on
        /// </summary>
        [BsonElement("created_on")]
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// The closed on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("closed_on")]
        public DateTimeOffset? ClosedOn { get; set; }

        /// <summary>
        /// The updated on
        /// </summary>
        [BsonElement("updated_on")]
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// The external created on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("external_created_on")]
        public DateTimeOffset? ExternalCreatedOn { get; set; }

        /// <summary>
        /// The external closed on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("external_closed_on")]
        public DateTimeOffset? ExternalClosedOn { get; set; }

        /// <summary>
        /// The bet
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("bet")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Bet { get; set; }

        /// <summary>
        /// The valid bet
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("valid_bet")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? ValidBet { get; set; }

        /// <summary>
        /// The settle
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("settle")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Settle { get; set; }

        /// <summary>
        /// The commission
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("commission")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Commission { get; set; }

        /// <summary>
        /// The turnover
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("turnover")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Turnover { get; set; }

        /// <summary>
        /// The win loss
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("winLoss")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? WinLoss { get; set; }

        /// <summary>
        /// The tie bet adjustment
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("tie_bet_adjustment")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? TieBetAdjustment { get; set; }

        /// <summary>
        /// The client status
        /// </summary>
        [BsonElement("client_status")]
        [BsonRepresentation(BsonType.Int32)]
        public int ClientStatus { get; set; }

        /// <summary>
        /// The reopen turnover adjustment
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("reopen_turnover_adjustment")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? ReopenTurnoverAdjustment { get; set; }

        /// <summary>
        /// The reopened on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("reopened_on")]
        public DateTimeOffset? ReopenedOn { get; set; }

        /// <summary>
        /// The additional data
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("additional_data")]
        public string? AdditionalData { get; set; } = default!;

        /// <summary>
        /// The record on
        /// </summary>
        [BsonElement("record_on")]
        public DateTimeOffset RecordOn { get; set; } = default!;
    }
}
