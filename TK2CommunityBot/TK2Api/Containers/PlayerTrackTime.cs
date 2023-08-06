namespace TK2Bot.API
{
    public struct PlayerTrackTime
    {
        public ApiPlayerId ApiPlayerId { get; internal init; }
        public TimeSpan RunTime { get; internal init; }
        public UInt32 Placement { get; internal set; }
    }
}