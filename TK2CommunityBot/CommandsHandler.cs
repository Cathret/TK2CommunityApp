using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace TK2Bot
{
    public class CommandsHandler : BaseCommandModule
    {
        [Command("ping")]
        private async Task PongCommand(CommandContext _context)
        {
            await _context.RespondAsync("Pong!");
        }
    }
}