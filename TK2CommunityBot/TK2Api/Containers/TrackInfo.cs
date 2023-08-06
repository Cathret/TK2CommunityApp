namespace TK2Bot.API
{
    public struct TrackInfo
    {
        public ETrackId TrackId { get; internal set; }
        public string WebMapShortUrl { get; internal init; }
    }
}
