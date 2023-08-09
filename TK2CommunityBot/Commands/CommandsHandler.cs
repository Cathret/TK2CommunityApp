using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TK2Bot.API;

namespace TK2Bot
{
    // [SuppressMessage("ReSharper", "UnusedMember.Local")] // Used from Command attribute
    public class CommandsHandler : ApplicationCommandModule
    {
        private const string TIMER_FORMAT = @"m\:ss\.fff";
        private const string FAKE_EMPTY_FIELD = "\u200B";
        
        [SlashCommand("ping", "Ping the bot and expect a response")]
        private async Task PongCommand(InteractionContext _context)
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }
        
        [SlashCommand("playerdetailed", "Get Player Detailed Info")]
        private async Task PlayerDetailedCommand(InteractionContext _context, [Option("PlayerName", "Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await CreatePlayerDetailedMessage(_playerName)));
        }
        
        [SlashCommand("player", "Get Player Info")]
        private async Task PlayerCommand(InteractionContext _context, [Option("PlayerName", "Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await CreatePlayerMessage(_playerName)));
        }

        private static async Task<DiscordMessageBuilder> CreatePlayerDetailedMessage(string _playerName)
        {
            FullPlayerInfo fullPlayerInfo = await ApiSystem.GetFullPlayerInfoFromName(_playerName);
            
            PlayerInfo    playerInfo    = fullPlayerInfo.PlayerInfo;
            PlayerRecords playerRecords = fullPlayerInfo.PlayerRecords;
            CountryInfo   countryInfo   = fullPlayerInfo.CountryInfo;
            ContinentInfo continentInfo = fullPlayerInfo.ContinentInfo;
            PlayerStats   playerStats   = fullPlayerInfo.PlayerStats;
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{playerInfo.PlayerName}'s Profile",
                Description = $"**{playerInfo.PlayerName}** is **{RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide)}** on the global leaderboard with **{RankingUtils.GetPrettyStringForPoints(playerStats.Points)} points**.\n",
                Url         = playerInfo.ProfileUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Green,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = playerInfo.AvatarUrl,
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text    = "https://the-karters-community.com/",
                    IconUrl = "https://the-karters-community.com/images/the-karters-logo.png"
                }
            };
            
            // embedBuilder.AddField(FAKE_EMPTY_FIELD, FAKE_EMPTY_FIELD, false);

            for (int trackIdx = 0; trackIdx < playerRecords.PlayerTrackTimes.Length; trackIdx++)
            {
                PlayerTrackTime oneTrackTime = playerRecords.PlayerTrackTimes[trackIdx];
                string formattedTime = oneTrackTime.RunTime.ToString(TIMER_FORMAT);
                string posWorld = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosWorldwide);
                string posContinent = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosContinent);
                string posCountry = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosCountry);

                string fieldDescription = $"**Time:** {formattedTime}\n" +
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
        
        private static async Task<DiscordMessageBuilder> CreatePlayerMessage(string _playerName)
        {
            FullPlayerInfo fullPlayerInfo = await ApiSystem.GetFullPlayerInfoFromName(_playerName);
            
            PlayerInfo    playerInfo    = fullPlayerInfo.PlayerInfo;
            PlayerRecords playerRecords = fullPlayerInfo.PlayerRecords;
            CountryInfo   countryInfo   = fullPlayerInfo.CountryInfo;
            ContinentInfo continentInfo = fullPlayerInfo.ContinentInfo;
            PlayerStats   playerStats   = fullPlayerInfo.PlayerStats;
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Title       = $"{playerInfo.PlayerName}'s Profile",
                Description = $"**{playerInfo.PlayerName}** is **{RankingUtils.GetPrettyStringForRank(playerStats.PosWorldwide)}** on the global leaderboard with **{RankingUtils.GetPrettyStringForPoints(playerStats.Points)} points**",
                Url         = playerInfo.ProfileUrl,
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Green,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = playerInfo.AvatarUrl,
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text    = "https://the-karters-community.com/",
                    IconUrl = "https://the-karters-community.com/images/the-karters-logo.png"
                }
            };
            string tableHeader = "\n" +"𒆜 **Time Trial Records** 𒆜\n\n";
           
            string tableContent = "";
            foreach (PlayerTrackTime oneTrackTime in playerRecords.PlayerTrackTimes)
            {
                string formattedTime = oneTrackTime.RunTime.ToString(TIMER_FORMAT);
                string posWorld = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosWorldwide);
                string posContinent = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosContinent);
                string posCountry = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosCountry);

                // row for different tracks
                tableContent += $" `☑️{oneTrackTime.TrackInfo.MapName}:`  {formattedTime} ⌛, {posCountry} :flag_{countryInfo.Alias.ToLower()}:, {posContinent} {RankingUtils.GetContinentEmojiTmp(continentInfo.Alias)}, {posWorld} :globe_with_meridians: \n";
            }

            // table header
            embedBuilder.Description += "\n" + tableHeader + tableContent;

            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build());
        }

        [SlashCommand("wr", "Request the WR info of a Track")]
        private async Task WrCommand(InteractionContext _context, [Option("Map", "Map for which we want the WR")] ETrackId _trackId)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await CreateWrMessage(_trackId)));
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
                Description = $"Current WR is held by **{playerInfo.PlayerName}** with a **{formattedDuration}**",
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