namespace TK2Bot.TK2RichPresenceTest
{
    internal static class Program
    {
        private const int WAIT_TIMER_MS = 1000 / 60;
        private static readonly string BOT_CLIENT_ID = Environment.GetEnvironmentVariable("TK2_BOT_CLIENT_ID")!;

        private static int Main()
        {
            DiscordHandler discordHandler = new DiscordHandler(long.Parse(BOT_CLIENT_ID));
            
            discordHandler.Initialize();

            bool updateSuccess = true;
            while (updateSuccess)
            {
                updateSuccess = discordHandler.RichPresenceUpdate();
                Thread.Sleep(WAIT_TIMER_MS);
            }

            return 0;
        }
    }
}