using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateTrackLeaderboardMessage(ETrackId _trackId, ELocation _location)
        {
            if (_location == ELocation.INVALID)
            {
                return GenerateErrorMessage("Invalid Filter");
            }
            
            TrackLeaderboard trackLeaderboard = await ApiSystem.GetTrackLeaderboard(_trackId, _location);
            
            TrackInfo trackInfo = trackLeaderboard.TrackInfo;
            
            string leaderboardHeader;
            string leaderboardContent = string.Empty;

            if (trackLeaderboard.LeaderboardRecords.Any())
            {
                leaderboardHeader = $"\n{TITLE_LEFT_EMOJI} **Leaderboards** {TITLE_RIGHT_EMOJI}\n\n";

                TrackLeaderboardEntry worseEntry = trackLeaderboard.LeaderboardRecords.Last();
                UInt32 maxPosLength = (uint)worseEntry.PlayerStats.PosWorldwide.ToString().Length;

                foreach (TrackLeaderboardEntry oneRecord in trackLeaderboard.LeaderboardRecords)
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
                    leaderboardContent += $" `⌛{formattedTime}` | :globe_with_meridians: `{posWorldwide}` | {continentEmoji} `{posContinent}` | {countryEmoji} `{posCountry}` - **{playerInfo.PlayerName}**\n";
                }
            }
            else
            {
                string noLeaderboardStr = "No Leaderboards found";
                if (_location != ELocation.NO_FILTER)
                {
                    noLeaderboardStr += $" for {LocationUtils.GetEmoji(_location)} {LocationUtils.GetName(_location)}";
                }
                leaderboardHeader = $"\n{TITLE_LEFT_EMOJI} **{noLeaderboardStr}** {TITLE_RIGHT_EMOJI}\n\n";
            }

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{trackInfo.MapName}'s Leaderboards",
                Description = $"\n{leaderboardHeader}{leaderboardContent}",
                Url         = trackInfo.LeaderboardUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Teal,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = trackInfo.ImageUrl,
                },
                Footer = EMBED_FOOTER
            };

            return GenerateEmbedMessage(embedBuilder);
        }
    }
}