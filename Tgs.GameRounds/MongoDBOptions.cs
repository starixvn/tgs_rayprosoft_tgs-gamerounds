namespace Tgs.GameRounds
{
    public record MongoDBOptions
    {
        public string ConnectionString { get; set; } = "mongodb://localhost";
    }
}
