using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateWrMessage(ETrackId _trackId, ELocation _location)
        {
            if (_location == ELocation.INVALID)
            {
                return GenerateErrorMessage("Invalid Filter");
            }
            
            WorldRecord worldRecord = await ApiSystem.GetWorldRecordForTrack(_trackId, _location);
            if (worldRecord.IsValid == false)
            {
                return GenerateErrorMessage($"API returned an error for Track **{MapTranslator.GetMapNameFromTrackId(_trackId)}** and Location **{LocationUtils.GetName(_location)}**.");
            }
            
            PlayerTrackTime wrTrackTime = worldRecord.WrTrackTime;
            PlayerInfo playerInfo = wrTrackTime.PlayerInfo;
            TrackInfo trackInfo = wrTrackTime.TrackInfo;

            string formattedDuration = wrTrackTime.RunTime.ToString(TIMER_FORMAT);

            string locationRecordTitle = "World Record";
            string locationRecordDesc = "WR";
            if (_location != ELocation.NO_FILTER)
            {
                locationRecordTitle = $"{LocationUtils.GetName(_location)}'s Record";
                string locationEmoji = $"{LocationUtils.GetEmoji(_location)}";
                locationRecordDesc = $"{locationEmoji}'s record";
            }

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name    = playerInfo.PlayerName,
                    Url     = playerInfo.ProfileUrl,
                    IconUrl = playerInfo.AvatarUrl,
                },
                Title       = $"{trackInfo.MapName}'s {locationRecordTitle}",
                Description = $"Current {locationRecordDesc} is held by **{playerInfo.PlayerName}** with a time of **{formattedDuration}**.",
                Url         = trackInfo.LeaderboardUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Blue,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url     = trackInfo.ImageUrl,
                },
                Footer      = EMBED_FOOTER
            };

            return GenerateEmbedMessage(embedBuilder);
        }
    }
}
