namespace TK2Bot.API
{
    public struct PlayerTrackTime
    {
        public PlayerInfo PlayerInfo { get; internal init; }
        public TrackInfo TrackInfo { get; internal init; }
        public TimeSpan RunTime { get; internal init; }
        public UInt32 Placement { get; internal set; }
    }
}