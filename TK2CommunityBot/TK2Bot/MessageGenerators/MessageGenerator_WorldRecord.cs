using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateWrMessage(ETrackId _trackId, ELocation _location)
        {
            if (LocationUtils.IsValidOption(_location) == false)
            {
                return new DiscordMessageBuilder()
                    .WithContent("Invalid Filter.");
            }
            
            PlayerTrackTime worldRecord = await ApiSystem.GetWorldRecordForTrack(_trackId, _location);
            PlayerInfo playerInfo = worldRecord.PlayerInfo;
            TrackInfo trackInfo = worldRecord.TrackInfo;

            string formattedDuration = worldRecord.RunTime.ToString(TIMER_FORMAT);

            string locationRecordTitle = "World Record";
            string locationRecordDesc = "WR";
            // TODO: use single GetEmoji function instead of splitting country/continent
            if ((_location & ELocation.COUNTRY) != ELocation.NO_FILTER)
            {
                locationRecordTitle = $"{LocationUtils.GetLocationNameFromLocationEnum(_location)}'s Record";
                string countryFlag = $":flag_{LocationUtils.GetAlias(_location)}:";
                locationRecordDesc = $"{countryFlag}'s record";
            }
            else if ((_location & ELocation.CONTINENT) != ELocation.NO_FILTER)
            {
                locationRecordTitle = $"{LocationUtils.GetLocationNameFromLocationEnum(_location)}'s Record";
                string continentEmoji = RankingUtils.GetContinentEmojiTmp(LocationUtils.GetAlias(_location).ToUpper());
                locationRecordDesc = $"{continentEmoji}'s record";
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

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}