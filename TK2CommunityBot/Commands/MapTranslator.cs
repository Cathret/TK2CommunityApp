using TK2Bot.API;

namespace TK2Bot
{
    public static partial class MapTranslator
    {
        public static ETrackId? GetTrackIdFromMapName(string _mapName)
        {
            return TRACK_TRANSLATION.TryGetValue(_mapName, out ETrackId trackId) ? trackId : null;
        }

        public static string GetMapNameFromTrackId(ETrackId _trackId)
        {
            return TRACK_TRANSLATION.First(_pair => _pair.Value == _trackId).Key;
        }

        public static IEnumerable<string> GetAllTrackNames()
        {
            return TRACK_TRANSLATION.Keys.ToArray();
        }

        public static UInt32 GetNumberOfTracks()
        {
            return (UInt32)TRACK_TRANSLATION.Count;
        }

        public static string GetSlugFromTrackId(ETrackId _trackId)
        {
            return SLUG_TRANSLATION.TryGetValue(_trackId, out string? mapSlug) ? mapSlug : string.Empty;
        }
    }
}