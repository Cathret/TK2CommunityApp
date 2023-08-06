using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Runtime.CompilerServices; // For removing Optim

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
        
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        [Command("wr")]
        private async Task WrCommand(CommandContext _context)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    Name = "Arca",
                    Url = "https://the-karters-community.com/player/415",
                    IconUrl = "https://the-karters-community.com/storage/users/1/avatar.jpg"
                },
                Title = "WORLD RECORD HOLDER",
                Description = "Current WR is held by Arca with a 1:11.548",
                Url = "https://the-karters-community.com/leaderboard/time-trial/woodsy-lane",
                ImageUrl = "",
                Timestamp = DateTimeOffset.UtcNow,
                Color = DiscordColor.Blue,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Url = "https://the-karters-community.com/images/tracks/woodsy-lane.png",
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter()
                {
                    Text = "https://www.youtube.com/watch?v=cnszxGaMmws",
                    IconUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/09/YouTube_full-color_icon_%282017%29.svg/1280px-YouTube_full-color_icon_%282017%29.svg.png"
                }
            };
            
            DiscordMessage discordMessage = await new DiscordMessageBuilder()
                .WithEmbed(embedBuilder.Build())
                .WithReply(_context.Message.Id)
                .SendAsync(_context.Channel);

            await discordMessage.RespondAsync("Answer myself");
        }
    }
}