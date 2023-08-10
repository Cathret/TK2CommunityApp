using DSharpPlus.Entities;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        private const string TIMER_FORMAT = @"m\:ss\.fff";
        private const string FAKE_EMPTY_FIELD = "\u200B";

        private const string TK2_LINK = "https://the-karters-community.com/";
        private const string TK2_LOGO = "https://the-karters-community.com/images/the-karters-logo.png";

        private static readonly DiscordEmbedBuilder.EmbedFooter EMBED_FOOTER = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text    = TK2_LINK,
            IconUrl = TK2_LOGO
        };
    }
}