using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace TK2Bot
{
    public abstract class Settings
    {
        public static readonly string BOT_TOKEN = Environment.GetEnvironmentVariable("TK2_BOT_TOKEN")!;
        
        public const string COMMAND_PREFIX      = "!tk2 ";
        public const LogLevel LOG_LEVEL         = LogLevel.Debug;
        public const DiscordIntents INTENTS     = DiscordIntents.Guilds | DiscordIntents.GuildMessageTyping | DiscordIntents.GuildMessages | DiscordIntents.GuildWebhooks | DiscordIntents.GuildIntegrations | DiscordIntents.MessageContents;
    }
}