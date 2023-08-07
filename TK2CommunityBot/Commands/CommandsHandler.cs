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
        private const string NO_BREAK_SPACE = "\u00A0";
        
        [SlashCommand("ping", "Ping the bot and expect a response")]
        private async Task PongCommand(InteractionContext _context)
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }
        
        [SlashCommand("player", "Get Player Info")]
        private async Task PlayerCommand(InteractionContext _context, [Option("PlayerName", "Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await CreatePlayerMessage(_playerName)));
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

            for (int trackIdx = 0; trackIdx < playerRecords.PlayerTrackTimes.Length; trackIdx++)
            {
                PlayerTrackTime oneTrackTime = playerRecords.PlayerTrackTimes[trackIdx];
                string formattedTime = oneTrackTime.RunTime.ToString(TIMER_FORMAT);
                string posWorld = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosWorldwide);
                string posContinent = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosContinent);
                string posCountry = RankingUtils.GetPrettyStringForRank(oneTrackTime.PlayerStats.PosCountry);

                string fieldDescription = $"**Time:**{NO_BREAK_SPACE}{formattedTime}\n" +
                                          //$"**Points:** {RankingUtils.GetPrettyStringForPoints(oneTrackTime.PlayerStats.Points)}\n" +
                                          $"\n" +
                                          $":globe_with_meridians:{NO_BREAK_SPACE}**Worldwide:**{NO_BREAK_SPACE}{posWorld}\n" +
                                          $"{RankingUtils.GetContinentEmojiTmp(continentInfo.Alias)}{NO_BREAK_SPACE}**{continentInfo.Name}:**{NO_BREAK_SPACE}{posContinent}\n" +
                                          $":flag_{countryInfo.Alias.ToLower()}:{NO_BREAK_SPACE}**{countryInfo.Name}:**{NO_BREAK_SPACE}{posCountry}\n";

                embedBuilder.AddField($"__{oneTrackTime.TrackInfo.MapName}__", fieldDescription, true);

                if (trackIdx % 2 == 1)
                {
                    // embedBuilder.AddField("---", "---", false);
                    // embedBuilder.AddField("", "", false); Crashshes app
                }
            }

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