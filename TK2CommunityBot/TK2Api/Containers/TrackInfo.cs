namespace TK2Bot.API
{
    public struct TrackInfo
    {
        public ETrackId TrackId { get; internal init; }
        public string Slug { get; internal init; }
        public string MapName { get; internal init; }
        public string ImageUrl { get; internal init; }
        public string LeaderboardUrl { get; internal init; }
    }
}
