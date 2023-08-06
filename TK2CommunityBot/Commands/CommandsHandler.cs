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
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(await CreateWrMessage(_trackId)));
        }

        private static async Task<DiscordMessageBuilder> CreateWrMessage(ETrackId _trackId)
        {
            PlayerTrackTime worldRecord = await ApiSystem.GetWorldRecordForTrack(_trackId);
            PlayerInfo playerInfo = worldRecord.PlayerInfo;
            TrackInfo trackInfo = worldRecord.TrackInfo;

            string formattedDuration = worldRecord.RunTime.ToString(@"m\:ss\.fff");
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name    = playerInfo.PlayerName,
                    Url     = playerInfo.ProfileUrl,
                    IconUrl = playerInfo.AvatarUrl,
                },
                Title       = "WORLD RECORD HOLDER",
                Description = $"Current WR is held by {playerInfo.PlayerName} with a {formattedDuration}",
                Url         = trackInfo.LeaderboardUrl,
                // ImageUrl = "https://www.youtube.com/watch?v=cnszxGaMmws", // Framework doesn't handle Youtube
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Blue,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url     = trackInfo.ImageUrl,
                },
                Footer      = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text    = trackInfo.LeaderboardUrl,
                    IconUrl = "https://the-karters-community.com/images/the-karters-logo.png"
                }
            };

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}