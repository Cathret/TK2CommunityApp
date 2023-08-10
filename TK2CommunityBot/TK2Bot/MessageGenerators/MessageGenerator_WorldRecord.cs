using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateWrMessage(ETrackId _trackId)
        {
            PlayerTrackTime worldRecord = await ApiSystem.GetWorldRecordForTrack(_trackId);
            PlayerInfo playerInfo = worldRecord.PlayerInfo;
            TrackInfo trackInfo = worldRecord.TrackInfo;

            string formattedDuration = worldRecord.RunTime.ToString(TIMER_FORMAT);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name    = playerInfo.PlayerName,
                    Url     = playerInfo.ProfileUrl,
                    IconUrl = playerInfo.AvatarUrl,
                },
                Title       = $"{trackInfo.MapName}'s World Record",
                Description = $"Current WR is held by **{playerInfo.PlayerName}** with a **{formattedDuration}**.",
                Url         = trackInfo.LeaderboardUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Blue,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url     = trackInfo.ImageUrl,
                },
                Footer      = EMBED_FOOTER
            };

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}