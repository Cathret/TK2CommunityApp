using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static DiscordMessageBuilder CreateLocationListMessage(string _search)
        {
            IEnumerable<ELocation> foundLocations = LocationUtils.SearchLocation(_search);
            if (!foundLocations.Any())
            {
                return GenerateErrorMessage($"Can't find any location with **{_search}**.");
            }
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"Location Search",
                Description = $"Found **{foundLocations.Count()} results** for **{_search}**:\n",
                Url         = TK2_LINK,
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Blurple,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = TK2_LOGO,
                },
                Footer = EMBED_FOOTER
            };

            string resultContent = string.Empty;
            foreach (ELocation oneLocation in foundLocations)
            {
                string locationName = LocationUtils.GetName(oneLocation);
                string locationAlias = LocationUtils.GetAlias(oneLocation);
                string locationEmoji = LocationUtils.GetEmoji(oneLocation);
            
                resultContent += $" {locationEmoji} `{locationAlias}` {locationName}\n";
            }

            embedBuilder.Description += "\n" + resultContent;

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}