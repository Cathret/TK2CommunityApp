namespace TK2Bot.API
{
    public struct ApiPlayerId
    {
        public Int32 WebPlayerId { get; internal init; }
        
        public override string ToString() => WebPlayerId.ToString();
    }
}