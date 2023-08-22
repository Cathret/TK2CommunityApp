namespace TK2Bot.API
{
    public struct PlayerTrackTime
    {
        public PlayerInfo PlayerInfo { get; internal init; }
        public TrackInfo TrackInfo { get; internal init; }
        public TimeSpan RunTime { get; internal init; }
        public PlayerStats PlayerStats { get; internal init; }
    }
}