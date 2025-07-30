namespace TK2Bot.TK2RichPresenceTest
{
    internal static class Program
    {
        private const bool DEBUG_DUAL_DISCORD = true;

        private const int WAIT_TIMER_MS = 1000 / 60;
        private const uint STEAM_APP_ID = 2269950;

        private static readonly string BOT_CLIENT_ID = Environment.GetEnvironmentVariable("TK2_BOT_CLIENT_ID")!;

        private static int Main()
        {
            return DEBUG_DUAL_DISCORD ? DebugDualDiscord() : RunSingleDiscord();
        }

        private static int RunSingleDiscord()
        {
            DiscordHandler discordHandler = new DiscordHandler(long.Parse(BOT_CLIENT_ID));
            discordHandler.Initialize(STEAM_APP_ID);

            bool updateSuccess = true;
            while (updateSuccess)
            {
                updateSuccess = discordHandler.RichPresenceUpdate();
                Thread.Sleep(WAIT_TIMER_MS);
            }

            return 0;
        }

        private static int DebugDualDiscord()
        {
            Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
            DiscordHandler discordStable = new DiscordHandler(long.Parse(BOT_CLIENT_ID));
            discordStable.Initialize(STEAM_APP_ID);

            Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "1");
            DiscordHandler discordCanary = new DiscordHandler(long.Parse(BOT_CLIENT_ID));
            discordCanary.Initialize(STEAM_APP_ID);

            bool updateStableSuccess = true;
            bool updateCanarySuccess = true;
            while (updateStableSuccess && updateCanarySuccess)
            {
                updateStableSuccess = discordStable.RichPresenceUpdate();
                updateCanarySuccess = discordCanary.RichPresenceUpdate();

                Thread.Sleep(WAIT_TIMER_MS);
            }

            return 0;
        }
    }
}
