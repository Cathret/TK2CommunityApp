using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreatePlayerMessage(string _playerName)
        {
            FullPlayerInfo fullPlayerInfo = await ApiSystem.GetFullPlayerInfoFromName(_playerName);
            
            if (fullPlayerInfo.IsValid == false)
            {
                return new DiscordMessageBuilder()
                    .WithContent($"Can't find player from PlayerName [{_playerName}]");
            }
            
            PlayerInfo    playerInfo    = fullPlayerInfo.PlayerInfo;
            PlayerRecords playerRecords = fullPlayerInfo.PlayerRecords;
            CountryInfo   countryInfo   = fullPlayerInfo.CountryInfo;
            ContinentInfo continentInfo = fullPlayerInfo.ContinentInfo;
            PlayerStats   playerStats   = fullPlayerInfo.PlayerStats;
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{playerInfo.PlayerName}'s Profile",
                Description = $"**{playerInfo.PlayerName}** is **{RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide)}** on the global leaderboard with **{RankingUtils.GetPrettyStringForPoints(playerStats.Points)} points**.",
                Url         = playerInfo.ProfileUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Green,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = playerInfo.AvatarUrl,
                },
                Footer = EMBED_FOOTER
            };
            string tableHeader = "\n𒆜 **Time Trial Records** 𒆜\n\n";

            ELocation countryLocation = LocationUtils.GetEnumFromName(countryInfo.Name) ?? ELocation.NO_FILTER;
            string countryEmoji = LocationUtils.GetEmoji(countryLocation);
            
            ELocation continentLocation = LocationUtils.GetEnumFromName(continentInfo.Name) ?? ELocation.NO_FILTER;
            string continentEmoji = LocationUtils.GetEmoji(continentLocation);
            
            string tableContent = "";
            foreach (PlayerTrackTime oneTrackTime in playerRecords.PlayerTrackTimes)
            {
                string formattedTime = oneTrackTime.RunTime.ToString(TIMER_FORMAT);
                string posWorld = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosWorldwide);
                string posContinent = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosContinent);
                string posCountry = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosCountry);

                // row for different tracks
                tableContent += $" `☑️{oneTrackTime.TrackInfo.MapName}:` ⌛{formattedTime}, {countryEmoji} {posCountry}, {continentEmoji} {posContinent}, :globe_with_meridians: {posWorld}\n";
            }

            // table header
            embedBuilder.Description += "\n" + tableHeader + tableContent;

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}