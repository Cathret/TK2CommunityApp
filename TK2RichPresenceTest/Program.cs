namespace TK2Bot.TK2RichPresenceTest
{
    internal static class Program
    {
        private static readonly string BOT_CLIENT_ID = Environment.GetEnvironmentVariable("TK2_BOT_CLIENT_ID")!;

        private static async Task Main()
        {
            DiscordHandler discordHandler = new DiscordHandler(long.Parse(BOT_CLIENT_ID));
            
            discordHandler.Initialize();

            bool updateSuccess = true;
            while (updateSuccess)
            {
                updateSuccess = discordHandler.RichPresenceUpdate();
                await Task.Delay(1000 / 60);
            }
        }
    }
}