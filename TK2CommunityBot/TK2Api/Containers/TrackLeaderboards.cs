namespace TK2Bot.API
{
    public struct TrackLeaderboards
    {
        public TrackInfo TrackInfo { get; internal init; }
        
        public TrackLeaderboardEntry[] LeaderboardRecords { get; internal init; }
    }
}
