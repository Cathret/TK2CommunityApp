namespace TK2Bot.API
{
    public struct PlayerInfo
    {
        public string PlayerName { get; internal init; }
        public ApiPlayerId ApiPlayerId { get; internal init; }
        public string KartersId { get; internal init; }
    }
}
