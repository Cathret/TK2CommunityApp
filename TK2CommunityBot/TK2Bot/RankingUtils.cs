using System.Globalization;

namespace TK2Bot
{
    public static class RankingUtils
    {
        private static readonly NumberFormatInfo POINTS_FORMAT_INFO = new NumberFormatInfo() { NumberGroupSeparator = " " }; 
        public static string GetPrettyStringForRank(UInt32 _rank)
        {
            UInt32 twoLastDigits = _rank % 100;
            if (twoLastDigits is 11 or 12 or 13) // The three irregulars
            {
                return $"{_rank}th";
            }
            
            UInt32 lastDigit = _rank % 10;
            return lastDigit switch
            {
                1 => $"{_rank}st",
                2 => $"{_rank}nd",
                3 => $"{_rank}rd",
                _ => $"{_rank}th"
            };
        }

        public static string GetPrettyStringForPoints(UInt32 _points)
        {
            return _points.ToString("N0", POINTS_FORMAT_INFO);
        }

        public static string GetContinentEmojiTmp(string _continentAlias)
        {
            return _continentAlias switch
            {
                "EU" => ":flag_eu:",              // EUROPE
                "AF" => ":earth_africa:",         // AMERICA
                "NA" => ":earth_americas:",       // NORTH AMERICA
                "SA" => ":earth_americas:",       // SOUTH AMERICA
                "OC" => ":earth_asia:",           // OCEANIA
                "AN" => ":globe_with_meridians:", // ANTARCTIC 
                _ => ":globe_with_meridians:"
            };
        }
    }
}
