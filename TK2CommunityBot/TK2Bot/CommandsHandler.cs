using System.Diagnostics.CodeAnalysis;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TK2Bot.API;

namespace TK2Bot
{
    // Avoid Warnings for Unused non-static members as they are used by SlashCommands
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("Performance", "CA1822:Marquer les membres comme étant static")]
    public class CommandsHandler : ApplicationCommandModule
    {
        [SlashCommand("ping", "Ping the bot and expect a response")]
        private async Task PongCommand(InteractionContext _context)
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Pong!"));
        }
        
        [SlashCommand("playerdetailed", "Get Player Detailed Info")]
        private async Task PlayerDetailedCommand(InteractionContext _context,
            [Option("PlayerName", "Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreatePlayerDetailedMessage(_playerName)));
        }
        
        [SlashCommand("player", "Get Player Info")]
        private async Task PlayerCommand(InteractionContext _context,
            [Option("PlayerName", "Name of the Player we want to retrieve the info")] string _playerName)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreatePlayerMessage(_playerName)));
        }

        [SlashCommand("wr", "Request the WR info of a Track")]
        private async Task WrCommand(InteractionContext _context,
            [Option("Map", "Map for which we want the WR")] ETrackId _trackId,
            [Option("LocationAliasFilter", "Filter to get information only for a specific location (eu, na, fr, gb..)")] string _locationAliasFilter = "")
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            ELocation locationFilter = LocationUtils.GetEnumFromAlias(_locationAliasFilter) ?? ELocation.NO_FILTER;
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateWrMessage(_trackId, locationFilter)));
        }

        [SlashCommand("mapleaderboards", "Request the Leaderboards of a Track")]
        private async Task MapLeaderboardsCommand(InteractionContext _context,
            [Option("Map", "Map for which we want the Leaderboards")] ETrackId _trackId,
            [Option("LocationAliasFilter", "Filter to get information only for a specific location (eu, na, fr, gb..)")] string _locationAliasFilter = "")
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            
            ELocation locationFilter = LocationUtils.GetEnumFromAlias(_locationAliasFilter) ?? ELocation.NO_FILTER;
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateTrackLeaderboardsMessage(_trackId, locationFilter)));
        }
        
        [SlashCommand("leaderboards", "Request the Global Leaderboards")]
        private async Task LeaderboardsCommand(InteractionContext _context,
            [Option("LocationAliasFilter", "Filter to get information only for a specific location (eu, na, fr, gb..)")] string _locationAliasFilter = "")
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            
            ELocation locationFilter = LocationUtils.GetEnumFromAlias(_locationAliasFilter) ?? ELocation.NO_FILTER;
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateGlobalLeaderboardsMessage(locationFilter)));
        }
        
        [SlashCommand("locationlist", "Get List of usable Alias to filter research")]
        private async Task LocationListCommand(InteractionContext _context,
            [Option("Search", "Part of the location looked up")] string _search)
        {
            await _context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            await _context.EditResponseAsync(new DiscordWebhookBuilder(await MessageGenerator.CreateLocationListMessage(_search)));
        }
    }
}