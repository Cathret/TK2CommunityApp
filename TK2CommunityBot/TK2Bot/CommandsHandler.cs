using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using DSharpPlus.Entities;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using TK2Bot.API;

namespace TK2Bot
{
    public static class CommandsHandler
    {
        [Command("ping")]
        [Description("Ping the bot and expect a response")]
        public static async ValueTask PongCommand(SlashCommandContext _context)
        {
            await _context.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }

        [Command("playerdetailed")]
        [Description("Get Player Detailed Info")]
        public static async ValueTask PlayerDetailedCommand(SlashCommandContext _context,
            [Parameter("PlayerName")] [Description("Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreatePlayerDetailedMessage(_playerName)));
        }

        [Command("player")]
        [Description("Get Player Info")]
        public static async ValueTask PlayerCommand(SlashCommandContext _context,
            [Parameter("PlayerName")] [Description("Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreatePlayerMessage(_playerName)));
        }

        [Command("wr")]
        [Description("Request the WR info of a Track")]
        public static async ValueTask WrCommand(SlashCommandContext _context,
            [Parameter("Map")] [Description("Map for which we want the WR")] ETrackId _trackId,
            [Parameter("Location")] [Description("Filter to get information only for a specific location")] [SlashAutoCompleteProvider<EnumAutoCompleteProvider<ELocation>>] ELocation _location = ELocation.INVALID)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateWrMessage(_trackId, _location)));
        }

        [Command("trackleaderboard")]
        [Description("Request the Leaderboard of a Track")]
        public static async ValueTask TrackLeaderboardCommand(SlashCommandContext _context,
            [Parameter("Track")] [Description("Track for which we want the Leaderboard")] ETrackId _trackId,
            [Parameter("Location")] [Description("Filter to get information only for a specific location")] [SlashAutoCompleteProvider<EnumAutoCompleteProvider<ELocation>>] ELocation _location = ELocation.INVALID)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateTrackLeaderboardMessage(_trackId, _location)));
        }

        [Command("leaderboard")]
        [Description("Request the Global Leaderboard")]
        public static async ValueTask LeaderboardCommand(SlashCommandContext _context,
            [Parameter("Location")] [Description("Filter to get information only for a specific location")] [SlashAutoCompleteProvider<EnumAutoCompleteProvider<ELocation>>] ELocation _location = ELocation.INVALID)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateGlobalLeaderboardMessage(_location)));
        }

        [Command("locationlist")]
        [Description("Get List of usable Alias to filter research")]
        public static async ValueTask LocationListCommand(SlashCommandContext _context,
            [Parameter("Search")] [Description("Part of the location looked up")] string _search)
        {
            await _context.DeferResponseAsync();
            await _context.EditResponseAsync(new DiscordWebhookBuilder(MessageGenerator.CreateLocationListMessage(_search)));
        }
    }
}
