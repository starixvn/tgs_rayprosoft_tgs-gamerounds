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
        [BsonRepresentation(BsonType.String)]
        public Guid RoundId { get; set; }

        /// <summary>
        /// The external round identifier
        /// </summary>
        [BsonElement("external_round_id")]
        [BsonRepresentation(BsonType.String)]
        public string ExternalRoundId { get; set; } = default!;

        /// <summary>
        /// The external game identifier
        /// </summary>
        [BsonElement("external_game_id")]
        [BsonRepresentation(BsonType.String)]
        public string ExternalGameId { get; set; } = default!;

        /// <summary>
        /// The game provider identifier
        /// </summary>
        [BsonElement("game_provider_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid GameProviderId { get; set; }

        /// <summary>
        /// The game provider code
        /// </summary>
        [BsonElement("game_provider_code")]
        public string GameProviderCode { get; set; } = default!;

        /// <summary>
        /// The game provider configuration identifier
        /// </summary>
        [BsonElement("game_provider_configuration_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid GameProviderConfigurationId { get; set; }

        /// <summary>
        /// The game identifier
        /// </summary>
        [BsonElement("game_id")]
        [BsonRepresentation(BsonType.Int32)]
        public int GameId { get; set; }

        /// <summary>
        /// The brand identifier
        /// </summary>
        [BsonElement("brand_id")]
        [BsonRepresentation(BsonType.String)]
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
        [BsonRepresentation(BsonType.String)]
        public Guid WalletId { get; set; }

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
        /// The client status
        /// </summary>
        [BsonElement("client_status")]
        [BsonRepresentation(BsonType.Int32)]
        public int ClientStatus { get; set; }

        /// <summary>
        /// The created on
        /// </summary>
        [BsonElement("created_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// The closed on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("closed_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? ClosedOn { get; set; }

        /// <summary>
        /// The updated on
        /// </summary>
        [BsonElement("updated_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset UpdatedOn { get; set; }

        /// <summary>
        /// The external created on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("external_created_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? ExternalCreatedOn { get; set; }

        /// <summary>
        /// The external closed on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("external_closed_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? ExternalClosedOn { get; set; }

        /// <summary>
        /// The bet
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonRepresentation(BsonType.String)]
        public decimal Bet { get; set; } = default!;

        /// <summary>
        /// The valid bet
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("valid_bet")]
        [BsonRepresentation(BsonType.String)]
        public decimal ValidBet { get; set; } = default!;

        /// <summary>
        /// The settle
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("settle")]
        [BsonRepresentation(BsonType.String)]
        public decimal Settle { get; set; } = default!;

        /// <summary>
        /// The commission
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("commission")]
        [BsonRepresentation(BsonType.String)]
        public decimal Commission { get; set; } = default!;

        /// <summary>
        /// The turnover
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("turnover")]
        [BsonRepresentation(BsonType.String)]
        public decimal Turnover { get; set; } = default!;

        /// <summary>
        /// The win loss
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("company_winloss")]
        [BsonRepresentation(BsonType.String)]
        public decimal CompanyWinLoss { get; set; } = default!;

        /// <summary>
        /// The tie bet adjustment
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("tie_bet_adjustment")]
        [BsonRepresentation(BsonType.String)]
        public decimal TieBetAdjustment { get; set; } = default!;

        /// <summary>
        /// The reopen turnover adjustment
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [BsonElement("reopen_turnover_adjustment")]
        [BsonRepresentation(BsonType.String)]
        public decimal ReopenTurnoverAdjustment { get; set; } = default!;

        /// <summary>
        /// The reopened on
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonElement("reopened_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? ReopenedOn { get; set; }

        /// <summary>
        /// The record on
        /// </summary>
        [BsonElement("record_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset RecordOn { get; set; } = default!;
    }
}
