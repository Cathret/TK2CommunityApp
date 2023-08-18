using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreateGlobalLeaderboardsMessage(ELocation _location)
        {
            return GenerateErrorMessage("Not implemented.");
            
            if (!Enum.IsDefined(typeof(ELocation), _location))
            {
                return new DiscordMessageBuilder()
                    .WithContent("Invalid Filter.");
            }
            
            GlobalLeaderboards globalLeaderboards = await ApiSystem.GetGlobalLeaderboards(_location);
            
            string leaderboardsHeader = string.Empty;
            string leaderboardsContent = string.Empty;

            if (globalLeaderboards.LeaderboardEntries.Any())
            {
                leaderboardsHeader = "\n𒆜 **Leaderboards** 𒆜\n\n";

                GlobalLeaderboardEntry bestEntry = globalLeaderboards.LeaderboardEntries.First();
                UInt32 maxPointsLength = (uint)bestEntry.PlayerStats.Points.ToString().Length;
                
                GlobalLeaderboardEntry worseEntry = globalLeaderboards.LeaderboardEntries.Last();
                UInt32 maxPosLength = (uint)worseEntry.PlayerStats.PosWorldwide.ToString().Length;

                foreach (GlobalLeaderboardEntry oneRecord in globalLeaderboards.LeaderboardEntries)
                {
                    PlayerInfo playerInfo = oneRecord.PlayerInfo;
                    ContinentInfo continentInfo = oneRecord.ContinentInfo;
                    CountryInfo countryInfo = oneRecord.CountryInfo;
                    PlayerStats playerStats = oneRecord.PlayerStats;

                    ELocation countryLocation = LocationUtils.GetEnumFromName(countryInfo.Name) ?? ELocation.NO_FILTER;
                    string countryEmoji = LocationUtils.GetEmoji(countryLocation);

                    ELocation continentLocation = LocationUtils.GetEnumFromName(continentInfo.Name) ?? ELocation.NO_FILTER;
                    string continentEmoji = LocationUtils.GetEmoji(continentLocation);

                    string points = RankingUtils.GetPrettyStringForPoints(playerStats.Points, maxPointsLength);
                    string posWorldwide = RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide, maxPosLength);
                    string posContinent = RankingUtils.GetPrettyStringForRank(playerStats.PosContinent, maxPosLength);
                    string posCountry = RankingUtils.GetPrettyStringForRank(playerStats.PosCountry, maxPosLength);
                    leaderboardsContent += $" `{points}` | :globe_with_meridians: `{posWorldwide}` | {continentEmoji} `{posContinent}` | {countryEmoji} `{posCountry}` - **{playerInfo.PlayerName}**\n";
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

            string embedLocationTitle = "Global";
            if (_location != ELocation.NO_FILTER)
            {
                embedLocationTitle = $"{LocationUtils.GetEmoji(_location)} {LocationUtils.GetName(_location)}'s";
            }
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{embedLocationTitle} Leaderboards",
                Description = $"\n{leaderboardsHeader}{leaderboardsContent}",
                Url         = TK2_LINK, // TODO: use global leaderboards link
                Timestamp   = DateTimeOffset.UtcNow,
                Color       = DiscordColor.Teal,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = TK2_LOGO,
                },
                Footer = EMBED_FOOTER
            };

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}