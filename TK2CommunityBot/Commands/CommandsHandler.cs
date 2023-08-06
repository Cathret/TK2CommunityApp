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
            await _context.RespondAsync("Usage: `!tk2 wr {MapName}` | No spaces in the name.");
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
            
            TrackInfo trackInfo = ApiSystem.GetTrackInfoFromId(trackId);
            PlayerTrackTime worldRecord = ApiSystem.GetWorldRecordForTrack(trackId);
            PlayerInfo playerInfo = ApiSystem.GetPlayerInfoFromId(worldRecord.ApiPlayerId);

            string formattedDuration = worldRecord.RunTime.ToString(@"m\:ss\.fff");
            
            Console.WriteLine($"https://the-karters-community.com/storage/users/{playerInfo.ApiPlayerId}/avatar.jpg");
            
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
            
            DiscordMessage discordMessage = await new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build())
                .AddComponents(components)
                .WithReply(_context.Message.Id)
                .SendAsync(_context.Channel);
        }
    }
}