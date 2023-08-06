namespace TK2Bot.API
{
    public struct PlayerInfo
    {
        public string PlayerName { get; internal init; }
        public ApiPlayerId ApiPlayerId { get; internal init; }
        public String KartersId { get; internal init; }
    }
}
