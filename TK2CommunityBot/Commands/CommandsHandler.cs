using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TK2Bot.API;

namespace TK2Bot
{
    // [SuppressMessage("ReSharper", "UnusedMember.Local")] // Used from Command attribute
    public class CommandsHandler : ApplicationCommandModule
    {
        [SlashCommand("ping", "Ping the bot and expect a response")]
        private async Task PongCommand(InteractionContext _context)
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }

        [SlashCommand("wr", "Request the WR info of a Track")]
        private async Task WrCommand(InteractionContext _context, [Option("Map", "Map for which we want the WR")] ETrackId _trackId)
        {
            //await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(CreateWrMessage(_trackId)));
        }

        public static DiscordMessageBuilder CreateWrMessage(ETrackId _trackId)
        {
            TrackInfo trackInfo = ApiSystem.GetTrackInfoFromId(_trackId);
            PlayerTrackTime worldRecord = ApiSystem.GetWorldRecordForTrack(_trackId);
            PlayerInfo playerInfo = ApiSystem.GetPlayerInfoFromId(worldRecord.ApiPlayerId);

            string formattedDuration = worldRecord.RunTime.ToString(@"m\:ss\.fff");
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name    = playerInfo.PlayerName,
                    Url     = $"https://the-karters-community.com/player/{playerInfo.KartersId}",
                    IconUrl = $"https://the-karters-community.com/storage/users/{playerInfo.ApiPlayerId}/avatar.jpg"
                },
                Title       = "WORLD RECORD HOLDER",
                Description = $"Current WR is held by {playerInfo.PlayerName} with a {formattedDuration}",
                Url         = $"https://the-karters-community.com/leaderboard/time-trial/{trackInfo.WebMapShortUrl}",
                // ImageUrl = "https://www.youtube.com/watch?v=cnszxGaMmws", // Framework doesn't handle Youtube
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Blue,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url     = $"https://the-karters-community.com/images/tracks/{trackInfo.WebMapShortUrl}.png",
                },
                Footer      = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text    = "https://www.youtube.com/watch?v=cnszxGaMmws",
                    IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/YouTube_full-color_icon_%282017%29.svg/1280px-YouTube_full-color_icon_%282017%29.svg.png"
                }
            };

            DiscordComponent[] components = new DiscordComponent[]
            {
                new DiscordLinkButtonComponent("https://www.youtube.com/watch?v=cnszxGaMmws", "Go Watch!"),
                // TODO: why not add a "compare" button?
            };
            
            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build())
                .AddComponents(components);
        }
    }
}