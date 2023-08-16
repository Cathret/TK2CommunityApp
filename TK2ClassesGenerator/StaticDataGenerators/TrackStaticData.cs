using System.Globalization;
using CsvHelper;

namespace TK2Bot.ClassesGenerator
{
    public static class TrackStaticData
    {
        private static readonly string TK2_GENERATED_MAPTRANSLATOR = DataGenInfo.TK2_GENERATED_DIR + "MapTranslator_Generated.cs";
        private static readonly string TK2_GENERATED_TRACKID = DataGenInfo.TK2_GENERATED_DIR + "ETrackId_Generated.cs";

        private const string TRACK_CSV_FILE = "tracks.csv";

        private class TrackInfo
        {
            public string? TrackName { get; set; }
            public string? SlugInfo { get; set; }
            public string? EnumName { get; set; }
            public string? ShortName { get; set; }
        }

        private static bool GenerateMapTranslatorData(IEnumerable<TrackInfo> _tracksInfo)
        {
            IEnumerable<TrackInfo> tracksInfo = _tracksInfo as TrackInfo[] ?? _tracksInfo.ToArray();
            
            string trackTranslationVar =
                "        private static readonly Dictionary<string, ETrackId> TRACK_TRANSLATION = new Dictionary<string, ETrackId>()\n" +
                "        {\n" +
                tracksInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ \"{_oneTrackInfo.TrackName}\", ETrackId.{_oneTrackInfo.EnumName} }},\n") +
                "        };\n";
            
            string slugTranslationVar =
                "        private static readonly Dictionary<ETrackId, string> SLUG_TRANSLATION = new Dictionary<ETrackId, string>()\n" +
                "        {\n" +
                tracksInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"            {{ ETrackId.{_oneTrackInfo.EnumName}, \"{_oneTrackInfo.SlugInfo}\" }},\n") +
                "        };\n";

            string translatorContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using TK2Bot.API;\n\n" +
                "namespace TK2Bot\n" +
                "{\n" +
                "    public static partial class MapTranslator\n" +
                "    {\n" +
                trackTranslationVar +
                "\n" +
                slugTranslationVar +
                "    }\n" +
                "}\n";
            
            using StreamWriter translatorWriter = new StreamWriter(TK2_GENERATED_MAPTRANSLATOR);
            translatorWriter.Write(translatorContent);

            return true;
        }

        private static bool GenerateEnumTrackId(IEnumerable<TrackInfo> _tracksInfo)
        {
            string trackIdContent =
                "// GENERATED FILE - DO NOT MODIFY //\n\n" +
                "using DSharpPlus.SlashCommands;\n\n" +
                "namespace TK2Bot.API\n" +
                "{\n" +
                "    public enum ETrackId\n" +
                "    {" +
                _tracksInfo.Aggregate("", (_current, _oneTrackInfo) => _current + $"\n        [ChoiceName(\"{_oneTrackInfo.TrackName}\")]\n        {_oneTrackInfo.EnumName},\n") +
                "    }\n" +
                "}";
            
            using StreamWriter trackIdWriter = new StreamWriter(TK2_GENERATED_TRACKID);
            trackIdWriter.Write(trackIdContent);

            return true;
        }
        
        public static bool Generate()
        {
            using StreamReader fileReader = new StreamReader(TRACK_CSV_FILE);
            using CsvReader csvReader = new CsvReader(fileReader, CultureInfo.InvariantCulture);

            IEnumerable<TrackInfo>? allTracksInfo = csvReader.GetRecords<TrackInfo>();
            if (allTracksInfo == null)
                return false;

            IEnumerable<TrackInfo> tracksInfo = allTracksInfo as TrackInfo[] ?? allTracksInfo.ToArray();
            GenerateMapTranslatorData(tracksInfo);
            GenerateEnumTrackId(tracksInfo);

            return true;
        }
    }
}