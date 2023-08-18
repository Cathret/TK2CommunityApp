using TK2Bot.API;

namespace TK2Bot
{
    public static partial class LocationUtils
    {
        public static bool IsContinent(string _locationName)
        {
            return LOCATION_TRANSLATION_CONTINENTS.ContainsKey(_locationName);
        }

        public static bool IsCountry(string _locationName)
        {
            return LOCATION_TRANSLATION_COUNTRIES.ContainsKey(_locationName);
        }
        
        public static bool IsContinent(ELocation _location)
        {
            return LOCATION_TRANSLATION_CONTINENTS.ContainsValue(_location);
        }

        public static bool IsCountry(ELocation _location)
        {
            return LOCATION_TRANSLATION_COUNTRIES.ContainsValue(_location);
        }
        
        public static string GetLocationTypeStr(string _location)
        {
            return IsContinent(_location)
                ? "continent"
                : IsCountry(_location)
                    ? "country"
                    : string.Empty;
        }
        
        public static string GetLocationTypeStr(ELocation _location)
        {
            return IsContinent(_location)
                ? "continent"
                : IsCountry(_location)
                    ? "country"
                    : string.Empty;
        }

        public static ELocation? GetLocationEnumFromContinent(string _continentName)
        {
            return LOCATION_TRANSLATION_CONTINENTS.TryGetValue(_continentName, out ELocation location) ? location : null;
        }
        
        public static ELocation? GetLocationEnumFromCountry(string _countryName)
        {
            return LOCATION_TRANSLATION_COUNTRIES.TryGetValue(_countryName, out ELocation location) ? location : null;
        }
        
        public static ELocation? GetEnum(string _locationName)
        {
            return IsContinent(_locationName)
                ? GetLocationEnumFromContinent(_locationName)
                : IsCountry(_locationName)
                    ? GetLocationEnumFromCountry(_locationName)
                    : ELocation.NO_FILTER;
        }

        public static string GetContinentName(ELocation _location)
        {
            return LOCATION_TRANSLATION_CONTINENTS.First(_pair => _pair.Value == _location).Key;
        }
        
        public static string GetCountryName(ELocation _location)
        {
            return LOCATION_TRANSLATION_COUNTRIES.First(_pair => _pair.Value == _location).Key;
        }
        
        public static string GetName(ELocation _location)
        {
            return IsContinent(_location)
                ? GetContinentName(_location)
                : IsCountry(_location)
                    ? GetCountryName(_location)
                    : string.Empty;
        }

        public static string GetContinentAlias(ELocation _location)
        {
            return ALIAS_TRANSLATION_CONTINENTS.TryGetValue(_location, out string? locationAlias) ? locationAlias : string.Empty;
        }
        
        public static string GetCountryAlias(ELocation _location)
        {
            return ALIAS_TRANSLATION_COUNTRIES.TryGetValue(_location, out string? locationAlias) ? locationAlias : string.Empty;
        }
        
        public static string GetAlias(ELocation _location)
        {
            return IsContinent(_location)
                ? GetContinentAlias(_location)
                : IsCountry(_location)
                    ? GetCountryAlias(_location)
                    : string.Empty;
        }
        
        public static string GetContinentEmoji(ELocation _location)
        {
            return EMOJI_TRANSLATION_CONTINENTS.TryGetValue(_location, out string? locationEmoji) ? locationEmoji : string.Empty;
        }
        
        public static string GetCountryEmoji(ELocation _location)
        {
            return EMOJI_TRANSLATION_COUNTRIES.TryGetValue(_location, out string? locationEmoji) ? locationEmoji : string.Empty;
        }
        
        public static string GetEmoji(ELocation _location)
        {
            return IsContinent(_location)
                ? GetContinentEmoji(_location)
                : IsCountry(_location)
                    ? GetCountryEmoji(_location)
                    : string.Empty;
        }
    }
}