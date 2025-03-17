using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace TK2Bot
{
    internal static class Program
    {
        private static async Task Main()
        {
            await API.ApiSystem.TryAuthentificate();

            DiscordClientBuilder discordClientBuilder = DiscordClientBuilder.CreateDefault(Settings.BOT_TOKEN, Settings.INTENTS);
            discordClientBuilder.SetLogLevel(Settings.LOG_LEVEL);
            discordClientBuilder.UseCommands(
                (_, _extension) =>
                {
                    _extension.AddCommands([typeof(CommandsHandler)]);
                },
                new CommandsConfiguration
                {
                    RegisterDefaultCommandProcessors = true,
                    UseDefaultCommandErrorHandler = true,
                }
            );

            DiscordClient discordClient = discordClientBuilder.Build();
            DiscordActivity activityStatus = new("Testing things", DiscordActivityType.Playing);
            await discordClient.ConnectAsync(activityStatus, DiscordUserStatus.Online);
            await Task.Delay(-1); // Avoid closing console
        }
    }
}
