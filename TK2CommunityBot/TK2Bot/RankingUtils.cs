using System.Globalization;

namespace TK2Bot
{
    public static class RankingUtils
    {
        private static readonly NumberFormatInfo POINTS_FORMAT_INFO = new NumberFormatInfo() { NumberGroupSeparator = " " };

        private static string GetNumberNoPad(UInt32 _rank)
        {
            return $"{_rank}";
        }
        
        private static string GetNumberPad1(UInt32 _rank)
        {
            return $"{_rank.ToString(),1}";
        }
        
        private static string GetNumberPad2(UInt32 _rank)
        {
            return $"{_rank.ToString(),2}";
        }
        
        private static string GetNumberPad3(UInt32 _rank)
        {
            return $"{_rank.ToString(),3}";
        }

        private static string GetNumberWithPad(UInt32 _rank, UInt32 _paddingLeft)
        {
            return _paddingLeft switch
            {
                1 => GetNumberPad1(_rank),
                2 => GetNumberPad2(_rank),
                3 => GetNumberPad3(_rank),
                _ => GetNumberNoPad(_rank)
            };
        }
        
        public static string GetPrettyStringForRank(UInt32 _rank, UInt32 _paddingLeft = 0)
        {
            UInt32 twoLastDigits = _rank % 100;
            if (twoLastDigits is 11 or 12 or 13) // The three irregulars
            {
                return $"{GetNumberWithPad(_rank, _paddingLeft)}th";
            }
            
            UInt32 lastDigit = _rank % 10;
            return lastDigit switch
            {
                1 => $"{GetNumberWithPad(_rank, _paddingLeft)}st",
                2 => $"{GetNumberWithPad(_rank, _paddingLeft)}nd",
                3 => $"{GetNumberWithPad(_rank, _paddingLeft)}rd",
                _ => $"{GetNumberWithPad(_rank, _paddingLeft)}th"
            };
        }

        public static string GetPrettyStringForPoints(UInt32 _points)
        {
            return _points.ToString("N0", POINTS_FORMAT_INFO);
        }
    }
}
