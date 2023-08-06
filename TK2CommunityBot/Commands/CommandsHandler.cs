using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using TK2Bot.API;

namespace TK2Bot
{
    // [SuppressMessage("ReSharper", "UnusedMember.Local")] // Used from Command attribute
    public class CommandsHandler : BaseCommandModule
    {
        [Command("ping")]
        private async Task PongCommand(CommandContext _context)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Description = "SomeDescription",
            };
            
            DiscordMessage discordMessage = await new DiscordMessageBuilder()
                .WithContent("Pong")
                .WithEmbed(embedBuilder)
                .WithReply(_context.Message.Id)
                .SendAsync(_context.Channel);
            
            // await _context.RespondAsync("Pong!");
        }

        [Command("wr")]
        private async Task WrCommand(CommandContext _context)
        {
            UInt32 nbTracks = MapTranslator.GetNumberOfTracks();
            string[]? allTracksName = MapTranslator.GetAllTrackNames() as string[];
            // Using buttons //
            //DiscordComponent[] components = new DiscordComponent[nbTracks];
            //for (int i = 0; i < nbTracks; i++)
            //{
            //    string oneTrackName = allTracksName![i];
            //    ButtonStyle buttonStyle = (i % 2 == 0) ? ButtonStyle.Primary : ButtonStyle.Secondary;
            //    
            //    components[i] = new DiscordButtonComponent(buttonStyle, oneTrackName, oneTrackName);
            //}
            ///// \Using Buttons
            
            // Using Select Dropdown //
            DiscordSelectComponentOption[] selectOptions = new DiscordSelectComponentOption[nbTracks];
            for (int i = 0; i < nbTracks; i++)
            {
                string oneTrackName = allTracksName![i];
                
                selectOptions[i] = new DiscordSelectComponentOption(oneTrackName, oneTrackName);
            }
            DiscordComponent[] components = new DiscordComponent[]
            {
                new DiscordSelectComponent("MapSelect", null!, selectOptions),
            };
            ///// \Using Select Dropdown
            
            await new DiscordMessageBuilder()
                .WithContent("Which World Record?")
                .AddComponents(components)
                .WithReply(_context.Message.Id)
                .SendAsync(_context.Channel);
        }

        [Command("wr")]
        private async Task WrCommand(CommandContext _context, string _mapName)
        {
            ETrackId trackId = MapTranslator.GetTrackIdFromMapName(_mapName);
            if (trackId == ETrackId.INVALID)
            {
                await _context.RespondAsync($"Can't find map using [{_mapName}]");
                return;
            }

            await CreateWrMessage(trackId)
                .WithReply(_context.Message.Id)
                .SendAsync(_context.Channel);
        }

        public static DiscordMessageBuilder CreateWrMessage(ETrackId _trackId)
        {
            TrackInfo trackInfo = ApiSystem.GetTrackInfoFromId(_trackId);
            PlayerTrackTime worldRecord = ApiSystem.GetWorldRecordForTrack(_trackId);
            PlayerInfo playerInfo = ApiSystem.GetPlayerInfoFromId(worldRecord.ApiPlayerId);

            string formattedDuration = worldRecord.RunTime.ToString(@"m\:ss\.fff");
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name    = playerInfo.PlayerName,
                    Url     = $"https://the-karters-community.com/player/{playerInfo.KartersId}",
                    IconUrl = $"https://the-karters-community.com/storage/users/{playerInfo.ApiPlayerId}/avatar.jpg"
                },
                Title       = "WORLD RECORD HOLDER",
                Description = $"Current WR is held by {playerInfo.PlayerName} with a {formattedDuration}",
                Url         = $"https://the-karters-community.com/leaderboard/time-trial/{trackInfo.WebMapShortUrl}",
                // ImageUrl = "https://www.youtube.com/watch?v=cnszxGaMmws", // Framework doesn't handle Youtube
                Timestamp   = DateTimeOffset.UtcNow,
                Color     = DiscordColor.Blue,
                Thumbnail   = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url     = $"https://the-karters-community.com/images/tracks/{trackInfo.WebMapShortUrl}.png",
                },
                Footer      = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text    = "https://www.youtube.com/watch?v=cnszxGaMmws",
                    IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/YouTube_full-color_icon_%282017%29.svg/1280px-YouTube_full-color_icon_%282017%29.svg.png"
                }
            };

            DiscordComponent[] components = new DiscordComponent[]
            {
                new DiscordLinkButtonComponent("https://www.youtube.com/watch?v=cnszxGaMmws", "Go Watch!"),
                // TODO: why not add a "compare" button?
            };
            
            return new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build())
                .AddComponents(components);
        }
    }
}