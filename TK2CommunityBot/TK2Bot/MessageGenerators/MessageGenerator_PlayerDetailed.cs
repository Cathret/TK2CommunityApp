using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MessageGenerator
    {
        public static async Task<DiscordMessageBuilder> CreatePlayerDetailedMessage(string _playerName)
        {
            FullPlayerInfo fullPlayerInfo = await ApiSystem.GetFullPlayerInfoFromName(_playerName);

            if (fullPlayerInfo.IsValid == false)
            {
                return new DiscordMessageBuilder()
                    .WithContent($"Can't find player from PlayerName [{_playerName}]");
            }

            PlayerInfo playerInfo = fullPlayerInfo.PlayerInfo;
            PlayerRecords playerRecords = fullPlayerInfo.PlayerRecords;
            CountryInfo countryInfo = fullPlayerInfo.CountryInfo;
            ContinentInfo continentInfo = fullPlayerInfo.ContinentInfo;
            PlayerStats playerStats = fullPlayerInfo.PlayerStats;

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title = $"{playerInfo.PlayerName}'s Profile",
                Description =
                    $"**{playerInfo.PlayerName}** is **{RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide)}** on the global leaderboard with **{RankingUtils.GetPrettyStringForPoints(playerStats.Points)} points**.\n",
                Url       = playerInfo.ProfileUrl,
                Timestamp = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Green,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = playerInfo.AvatarUrl,
                },
                Footer = EMBED_FOOTER
            };

            // embedBuilder.AddField(FAKE_EMPTY_FIELD, FAKE_EMPTY_FIELD, false);

            for (int trackIdx = 0; trackIdx < playerRecords.PlayerTrackTimes.Length; trackIdx++)
            {
                PlayerTrackTime oneTrackTime = playerRecords.PlayerTrackTimes[trackIdx];
                string formattedTime = oneTrackTime.RunTime.ToString(TIMER_FORMAT);
                string posWorld = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosWorldwide);
                string posContinent = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosContinent);
                string posCountry = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosCountry);

                string fieldDescription = $"⌛ **Time:** {formattedTime}\n" +
                                          $"**Points:** {RankingUtils.GetPrettyStringForPoints(oneTrackTime.PlayerStats.Points)}\n" +
                                          $"\n" +
                                          $":globe_with_meridians: **Worldwide:** {posWorld}\n" +
                                          $"{RankingUtils.GetContinentEmojiTmp(continentInfo.Alias)} **{continentInfo.Name}:** {posContinent}\n" +
                                          $":flag_{countryInfo.Alias.ToLower()}: **{countryInfo.Name}:** {posCountry}\n";

                embedBuilder.AddField($"__{oneTrackTime.TrackInfo.MapName}__", fieldDescription, true);

                if (trackIdx % 2 == 1)
                {
                    embedBuilder.AddField(FAKE_EMPTY_FIELD, FAKE_EMPTY_FIELD, false);
                }
            }

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }
    }
}