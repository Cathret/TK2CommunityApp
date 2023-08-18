using System.Globalization;

namespace TK2Bot
{
    public static class RankingUtils
    {
        private static readonly NumberFormatInfo POINTS_FORMAT_INFO = new NumberFormatInfo() { NumberGroupSeparator = " " };

        private static string GetNumberWithPad(UInt32 _number, UInt32 _paddingLeft)
        {
            return _paddingLeft == 0 ? _number.ToString() : _number.ToString().PadLeft((int)_paddingLeft);
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

        public static string GetPrettyStringForPoints(UInt32 _points, UInt32 _paddingLeft = 0)
        {
            int truePadding = (int)(_paddingLeft + Math.Floor(((decimal)_paddingLeft / 3)));
            return _points.ToString("N0", POINTS_FORMAT_INFO).PadLeft(truePadding);
        }
    }
}
