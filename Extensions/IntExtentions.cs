using System.Collections.Generic;
using System.Globalization;

namespace Extensions {
    public static class IntExtentions {
        public static string AddZeros(this int value,
                                      int max) {
            var txtValue = value.ToString(CultureInfo.InvariantCulture);
            var maxLen = max.ToString(CultureInfo.InvariantCulture)
                            .Length;
            var repLen = maxLen - txtValue.Length;
            return repLen <= 0
                       ? txtValue
                       : $"{new string('0', repLen)}{txtValue}";
        }

        public static List<int> ToList(this int value) {
            return new List<int> {value};
        }
    }
}