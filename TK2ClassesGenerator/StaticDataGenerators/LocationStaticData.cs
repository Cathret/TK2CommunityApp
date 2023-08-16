using System.Globalization;
using CsvHelper;

namespace TK2Bot.ClassesGenerator
{
    public static class LocationStaticData
    {
        private static readonly string TK2_GENERATED_LOCATIONUTILS = DataGenInfo.TK2_GENERATED_DIR + "LocationUtils_Generated.cs";
        private static readonly string TK2_GENERATED_LOCATION = DataGenInfo.TK2_GENERATED_DIR + "ELocation_Generated.cs";

        private const string CONTINENT_CSV_FILE = "continents.csv";
        private const string COUNTRIES_CSV_FILE = "countries.csv";

        private class LocationInfo
        {
            public string? Name { get; set; }
            public string? Alias { get; set; }
            public string? Enum { get; set; }
            public string? EmojiStr { get; set; }
        }

        private static bool GenerateLocationUtilsData(IEnumerable<LocationInfo> _countriesInfo, IEnumerable<LocationInfo> _continentsInfo)
        {
            IEnumerable<LocationInfo> countriesInfo = _countriesInfo as LocationInfo[] ?? _countriesInfo.ToArray();
            IEnumerable<LocationInfo> continentsInfo = _continentsInfo as LocationInfo[] ?? _continentsInfo.ToArray();
            
            string locationTranslationVar =
                "        private static readonly Dictionary<string, ELocation> LOCATION_TRANSLATION = new Dictionary<string, ELocation>()\n" +
                "        {\n" +
                countriesInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ \"{_oneTrackInfo.Name}\", ELocation.{_oneTrackInfo.Enum} }},\n") +
                continentsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ \"{_oneTrackInfo.Name}\", ELocation.{_oneTrackInfo.Enum} }},\n") +
                "        };\n";
            
            string aliasTranslationVar =
                "        private static readonly Dictionary<ELocation, string> ALIAS_TRANSLATION = new Dictionary<ELocation, string>()\n" +
                "        {\n" +
                countriesInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.Alias}\" }},\n") +
                continentsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.Alias}\" }},\n") +
                "        };\n";
            
            string emojiTranslationVar =
                "        private static readonly Dictionary<ELocation, string> EMOJI_TRANSLATION = new Dictionary<ELocation, string>()\n" +
                "        {\n" +
                countriesInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.EmojiStr}\" }},\n") +
                continentsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.EmojiStr}\" }},\n") +
                "        };\n";

            string translatorContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using TK2Bot.API;\n\n" +
                "namespace TK2Bot\n" +
                "{\n" +
                "    public static partial class LocationUtils\n" +
                "    {\n" +
                locationTranslationVar +
                "\n" +
                aliasTranslationVar +
                "\n" +
                emojiTranslationVar +
                "    }\n" +
                "}\n";
            
            using StreamWriter translatorWriter = new StreamWriter(TK2_GENERATED_LOCATIONUTILS);
            translatorWriter.Write(translatorContent);

            return true;
        }

        private static bool GenerateEnumLocation(IEnumerable<LocationInfo> _countriesInfo, IEnumerable<LocationInfo> _continentsInfo)
        {
            IEnumerable<LocationInfo> countriesInfo = _countriesInfo as LocationInfo[] ?? _countriesInfo.ToArray();
            IEnumerable<LocationInfo> continentsInfo = _continentsInfo as LocationInfo[] ?? _continentsInfo.ToArray();

            string countryMask = $"        COUNTRY = {countriesInfo.ElementAt(0).Enum}";
            for (int i = 1; i < countriesInfo.Count(); i++)
            {
                LocationInfo oneLocationInfo = countriesInfo.ElementAt(i);
                countryMask += $" | {oneLocationInfo.Enum}";
            }
            countryMask += ",\n";

            string continentMask = $"        CONTINENT = {continentsInfo.ElementAt(0).Enum}";
            for (int i = 1; i < continentsInfo.Count(); i++)
            {
                LocationInfo oneLocationInfo = continentsInfo.ElementAt(i);
                continentMask += $" | {oneLocationInfo.Enum}";
            }
            continentMask += ",\n";

            Int32 bitShift = -1;
            
            string locationEnumContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using DSharpPlus.SlashCommands;\n\n" +
                "namespace TK2Bot.API\n" +
                "{\n" +
                "    [Flags]\n" +
                "    public enum ELocation\n" +
                "    {\n" +
                "        [ChoiceName(\"None\")]\n" +
                "        NO_FILTER = 0,\n" +
                countriesInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"\n        [ChoiceName(\"Country: {_oneTrackInfo.Name}\")]\n        {_oneTrackInfo.Enum} = 1 << {(++bitShift).ToString()},\n") +
                continentsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"\n        [ChoiceName(\"Continent: {_oneTrackInfo.Name}\")]\n        {_oneTrackInfo.Enum} = 1 << {(++bitShift).ToString()},\n") +
                "\n" +
                countryMask +
                continentMask +
                "    }\n" +
                "}";
            
            using StreamWriter trackIdWriter = new StreamWriter(TK2_GENERATED_LOCATION);
            trackIdWriter.Write(locationEnumContent);

            return true;
        }
        
        public static bool Generate()
        {
            using StreamReader countryFileReader = new StreamReader(COUNTRIES_CSV_FILE);
            using CsvReader csvCountryReader = new CsvReader(countryFileReader, CultureInfo.InvariantCulture);
            IEnumerable<LocationInfo>? allCountriesInfo = csvCountryReader.GetRecords<LocationInfo>();
            if (allCountriesInfo == null)
                return false;
            
            using StreamReader continentFileReader = new StreamReader(CONTINENT_CSV_FILE);
            using CsvReader csvContinentReader = new CsvReader(continentFileReader, CultureInfo.InvariantCulture);
            IEnumerable<LocationInfo>? allContinentsInfo = csvContinentReader.GetRecords<LocationInfo>();
            if (allContinentsInfo == null)
                return false;

            IEnumerable<LocationInfo> countriesInfo = allCountriesInfo as LocationInfo[] ?? allCountriesInfo.ToArray();
            IEnumerable<LocationInfo> continentsInfo = allContinentsInfo as LocationInfo[] ?? allContinentsInfo.ToArray();
            
            GenerateLocationUtilsData(countriesInfo, continentsInfo);
            GenerateEnumLocation(countriesInfo, continentsInfo);

            return true;
        }
    }
}