namespace TK2Bot.API
{
    public struct LeaderboardRecord
    {
        public PlayerInfo PlayerInfo { get; internal init; }
        public ContinentInfo ContinentInfo { get; internal init; }
        public CountryInfo CountryInfo { get; internal init; }
        public PlayerStats PlayerStats { get; internal init; }
        public TimeSpan RunTime { get; internal init; }
    }
}
