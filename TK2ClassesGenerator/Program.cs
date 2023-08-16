namespace TK2Bot.ClassesGenerator
{
    internal static class Program
    {
        private static int Main(string[] _args)
        {
            TrackStaticData.Generate();
            LocationStaticData.Generate();
            
            return 0;
        }
    }
}