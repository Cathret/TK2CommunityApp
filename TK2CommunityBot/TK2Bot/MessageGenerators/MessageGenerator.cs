using DSharpPlus.Entities;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        private const string TIMER_FORMAT = @"m\:ss\.fff";
        private const string FAKE_EMPTY_FIELD = "\u200B";

        private const string TITLE_LEFT_EMOJI = "<:pigeonCoolFlipped:1143158497857507479>";
        private const string TITLE_RIGHT_EMOJI = "<:pigeonCool:1130943265705832608>";

        private const string TK2_LINK = "https://the-karters-community.com/";
        private const string TK2_LOGO = "https://the-karters-community.com/images/the-karters-logo.png";

        private static readonly DiscordEmbedBuilder.EmbedFooter EMBED_FOOTER = new DiscordEmbedBuilder.EmbedFooter()
        {
            Text    = TK2_LINK,
            IconUrl = TK2_LOGO
        };

        // Used for Single Embed Messages
        private static DiscordMessageBuilder GenerateEmbedMessage(DiscordEmbed _embed)
        {
            return new DiscordMessageBuilder()
                .AddEmbeds([_embed]);
        }

        private static DiscordMessageBuilder GenerateErrorMessage(string _content)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"Command Error",
                Description = $"**__Error:__** {_content}",
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Red,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = TK2_LOGO,
                },
                Footer = EMBED_FOOTER
            };

            return GenerateEmbedMessage(embedBuilder);
        }
    }
}