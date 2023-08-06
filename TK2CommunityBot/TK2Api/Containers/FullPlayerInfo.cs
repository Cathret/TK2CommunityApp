namespace TK2Bot.API
{
    public struct FullPlayerInfo
    {
        public PlayerInfo PlayerInfo { get; internal init; }
        public ContinentInfo ContinentInfo { get; internal init; }
        public CountryInfo CountryInfo { get; internal init; }
        public PlayerRecords PlayerRecords { get; internal init; }
        public PlayerStats PlayerStats { get; internal init; }
    }
}
