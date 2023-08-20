namespace TK2Bot.API
{
    public class GlobalLeaderboard
    {
        public PaginationInformation PaginationInfo { get; internal init; } = null!;
        public GlobalLeaderboardEntry[] LeaderboardEntries { get; internal init; } = null!;
    }
}
