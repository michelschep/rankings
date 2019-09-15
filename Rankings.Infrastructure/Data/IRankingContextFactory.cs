namespace Rankings.Infrastructure.Data
{
    public interface IRankingContextFactory
    {
        RankingContext Create(string connectionString);
    }
}