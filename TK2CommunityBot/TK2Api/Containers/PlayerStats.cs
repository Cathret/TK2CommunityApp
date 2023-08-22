namespace TK2Bot.API
{
    public struct PlayerStats
    {
        public UInt32 PosWorldwide { get; internal init; }
        public UInt32 PosContinent { get; internal init; }
        public UInt32 PosCountry { get; internal init; }
        public UInt32 Points { get; internal init; }
    }
}
