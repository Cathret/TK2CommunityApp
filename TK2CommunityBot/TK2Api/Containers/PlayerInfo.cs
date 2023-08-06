namespace TK2Bot.API
{
    public struct PlayerInfo
    {
        public string PlayerName { get; internal init; }
        public string ProfileUrl { get; internal init; }
        public string AvatarUrl { get; internal init; }
        // TODO: add continent + country
    }
}
