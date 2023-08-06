using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace TK2Bot
{
    internal static class Program
    {
        private static async Task Main(string[] _args)
        {
            DiscordClient discordClient = new(new DiscordConfiguration()
            {
                TokenType       = TokenType.Bot,
                Token           = Settings.BOT_TOKEN,
                Intents         = Settings.INTENTS,
                MinimumLogLevel = Settings.LOG_LEVEL,
            });
            
            CommandsNextExtension globalCommandsHandler = discordClient.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes  = new[] { Settings.COMMAND_PREFIX },
            });

            discordClient.ComponentInteractionCreated += async (s, e) =>
            {
                API.ETrackId trackId = MapTranslator.GetTrackIdFromMapName(e.Id);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(CommandsHandler.CreateWrMessage(trackId).WithContent("")));
            };

            globalCommandsHandler.RegisterCommands<CommandsHandler>();

            await discordClient.ConnectAsync();
            await Task.Delay(-1); // Avoid closing console
        }
    }
}