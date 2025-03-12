using System.Globalization;
using CsvHelper;

namespace TK2Bot.ClassesGenerator
{
    public static class LocationStaticData
    {
        private static readonly string TK2_GENERATED_LOCATIONUTILS = DataGenInfo.TK2_GENERATED_DIR + "LocationUtils_Generated.cs";
        private static readonly string TK2_GENERATED_LOCATION = DataGenInfo.TK2_GENERATED_DIR + "ELocation_Generated.cs";

        private const string CONTINENT_CSV_FILE = DataGenInfo.STATIC_DATA_DIR + "continents.csv";
        private const string COUNTRIES_CSV_FILE = DataGenInfo.STATIC_DATA_DIR + "countries.csv";

        private const string COUNTRIES_ENUM_NAME = "COUNTRIES";
        private const string CONTINENTS_ENUM_NAME = "CONTINENTS";

        private class LocationInfo
        {
            public string? Name { get; set; }
            public string? Alias { get; set; }
            public string? Enum { get; set; }
            public string? EmojiStr { get; set; }
        }

        private static string CreateDictionaries(string _locationName, IEnumerable<LocationInfo> _locationsInfo)
        {
            IEnumerable<LocationInfo> locationsInfo = _locationsInfo as LocationInfo[] ?? _locationsInfo.ToArray();

            string locationTranslationVar =
                $"        private static readonly Dictionary<string, ELocation> LOCATION_TRANSLATION_{_locationName} = new Dictionary<string, ELocation>()\n" +
                "        {\n" +
                locationsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ \"{_oneTrackInfo.Name}\", ELocation.{_oneTrackInfo.Enum} }},\n") +
                "        };\n";
            
            string aliasTranslationVar =
                $"        private static readonly Dictionary<ELocation, string> ALIAS_TRANSLATION_{_locationName} = new Dictionary<ELocation, string>()\n" +
                "        {\n" +
                locationsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.Alias}\" }},\n") +
                "        };\n";
            
            string emojiTranslationVar =
                $"        private static readonly Dictionary<ELocation, string> EMOJI_TRANSLATION_{_locationName} = new Dictionary<ELocation, string>()\n" +
                "        {\n" +
                locationsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ELocation.{_oneTrackInfo.Enum}, \"{_oneTrackInfo.EmojiStr}\" }},\n") +
                "        };\n";

            return locationTranslationVar + "\n" + aliasTranslationVar + "\n" + emojiTranslationVar;
        }

        private static bool GenerateLocationUtilsData(IEnumerable<LocationInfo> _countriesInfo, IEnumerable<LocationInfo> _continentsInfo)
        {
            IEnumerable<LocationInfo> countriesInfo = _countriesInfo as LocationInfo[] ?? _countriesInfo.ToArray();
            IEnumerable<LocationInfo> continentsInfo = _continentsInfo as LocationInfo[] ?? _continentsInfo.ToArray();

            string countriesDictionaries = CreateDictionaries(COUNTRIES_ENUM_NAME, countriesInfo);
            string continentsDictionaries = CreateDictionaries(CONTINENTS_ENUM_NAME, continentsInfo);

            string translatorContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using TK2Bot.API;\n\n" +
                "namespace TK2Bot\n" +
                "{\n" +
                "    public static partial class LocationUtils\n" +
                "    {\n" +
                countriesDictionaries +
                "\n" +
                continentsDictionaries +
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

            string locationEnumContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;\n\n" +
                "namespace TK2Bot.API\n" +
                "{\n" +
                "    public enum ELocation\n" +
                "    {\n" +
                "        [ChoiceDisplayName(\"None\")]\n" +
                "        NO_FILTER,\n" +
                countriesInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"\n        [ChoiceDisplayName(\"Country: {_oneTrackInfo.Name}\")]\n        {_oneTrackInfo.Enum},\n") +
                continentsInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"\n        [ChoiceDisplayName(\"Continent: {_oneTrackInfo.Name}\")]\n        {_oneTrackInfo.Enum},\n") +
                "\n" +
                "        [ChoiceDisplayName(\"Invalid\")]\n" +
                "        INVALID,\n" +
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