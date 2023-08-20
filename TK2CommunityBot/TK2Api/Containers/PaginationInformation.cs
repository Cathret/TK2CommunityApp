namespace TK2Bot.API
{
    public class PaginationInformation
    {
        public uint ItemsCount { get; internal init; }
        public uint ItemsTotalCount { get; internal init; }
        public uint Page { get; internal init; }
        public uint LastPage { get; internal init; }
    }
}
