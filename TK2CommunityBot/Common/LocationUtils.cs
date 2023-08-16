using TK2Bot.API;

namespace TK2Bot
{
    public static partial class LocationUtils
    {
        public static ELocation? GetLocationEnumFromLocationName(string _locationName)
        {
            return LOCATION_TRANSLATION.TryGetValue(_locationName, out ELocation location) ? location : null;
        }

        public static string GetLocationNameFromLocationEnum(ELocation _location)
        {
            return LOCATION_TRANSLATION.First(_pair => _pair.Value == _location).Key;
        }

        public static IEnumerable<string> GetAllLocations()
        {
            return LOCATION_TRANSLATION.Keys.ToArray();
        }

        public static UInt32 GetNumberOfLocations()
        {
            return (UInt32)LOCATION_TRANSLATION.Count;
        }

        public static string GetAlias(ELocation _location)
        {
            return ALIAS_TRANSLATION.TryGetValue(_location, out string? locationAlias) ? locationAlias : string.Empty;
        }
        
        public static string GetEmoji(ELocation _location)
        {
            return EMOJI_TRANSLATION.TryGetValue(_location, out string? locationEmoji) ? locationEmoji : string.Empty;
        }

        public static bool IsValidOption(ELocation _location)
        {
            return _location != ELocation.COUNTRY && _location != ELocation.CONTINENT;
        }
    }
}