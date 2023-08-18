using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateTrackLeaderboardsMessage(ETrackId _trackId, ELocation _location)
        {
            if (!Enum.IsDefined(typeof(ELocation), _location))
            {
                return new DiscordMessageBuilder()
                    .WithContent("Invalid Filter.");
            }
            
            TrackLeaderboards trackLeaderboards = await ApiSystem.GetTrackLeaderboards(_trackId, _location);
            
            TrackInfo trackInfo = trackLeaderboards.TrackInfo;
            
            string leaderboardsHeader = string.Empty;
            string leaderboardsContent = string.Empty;

            if (trackLeaderboards.LeaderboardRecords.Any())
            {
                leaderboardsHeader = "\n𒆜 **Leaderboards** 𒆜\n\n";

                LeaderboardRecord worseRecord = trackLeaderboards.LeaderboardRecords.Last();
                UInt32 maxPosLength = (uint)worseRecord.PlayerStats.PosWorldwide.ToString().Length;

                foreach (LeaderboardRecord oneRecord in trackLeaderboards.LeaderboardRecords)
                {
                    PlayerInfo playerInfo = oneRecord.PlayerInfo;
                    ContinentInfo continentInfo = oneRecord.ContinentInfo;
                    CountryInfo countryInfo = oneRecord.CountryInfo;
                    PlayerStats playerStats = oneRecord.PlayerStats;

                    string formattedTime = oneRecord.RunTime.ToString(TIMER_FORMAT);

                    ELocation countryLocation = LocationUtils.GetEnumFromName(countryInfo.Name) ?? ELocation.NO_FILTER;
                    string countryEmoji = LocationUtils.GetEmoji(countryLocation);

                    ELocation continentLocation = LocationUtils.GetEnumFromName(continentInfo.Name) ?? ELocation.NO_FILTER;
                    string continentEmoji = LocationUtils.GetEmoji(continentLocation);

                    string posWorldwide = RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide, maxPosLength);
                    string posContinent = RankingUtils.GetPrettyStringForRank(playerStats.PosContinent, maxPosLength);
                    string posCountry = RankingUtils.GetPrettyStringForRank(playerStats.PosCountry, maxPosLength);
                    leaderboardsContent += $" `⌛{formattedTime}` | :globe_with_meridians: `{posWorldwide}` | {continentEmoji} `{posContinent}` | {countryEmoji} `{posCountry}` - **{playerInfo.PlayerName}**\n";
                }
            }
            else
            {
                string noLeaderboardStr = "No Leaderboards found";
                if (_location != ELocation.NO_FILTER)
                {
                    noLeaderboardStr += $" for {LocationUtils.GetEmoji(_location)} {LocationUtils.GetName(_location)}";
                }
                leaderboardsHeader = $"\n𒆜 **{noLeaderboardStr}** 𒆜\n\n";
            }

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{trackInfo.MapName}'s Leaderboards",
                Description = $"\n{leaderboardsHeader}{leaderboardsContent}",
                Url         = trackInfo.LeaderboardUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Teal,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = trackInfo.ImageUrl,
                },
                Footer = EMBED_FOOTER
            };

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}