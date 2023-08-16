namespace TK2Bot.ClassesGenerator
{
    public static class DataGenInfo
    {
        private static readonly string TK2_BOT_PROJECT_DIR = Environment.GetEnvironmentVariable("TK2_COMMUNITY_BOT_PROJECT")!;
        public static readonly string TK2_GENERATED_DIR = TK2_BOT_PROJECT_DIR + "Generated\\";
    }
}
