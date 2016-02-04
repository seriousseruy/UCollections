using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace Extensions {
    public static class StringExtension {
        private static DateTime _dateTimeMinValue = DateTime.Parse("1/1/1900");

        public static T DeserializeObject<T>(this string str) where T : class {
            if (str.IsEmpty()) {
                return default(T);
            }

            try {
                return JsonConvert.DeserializeObject<T>(str);
            } catch (Exception) {
                return default(T);
            }
        }

        public static string CorrectFileName(this string str) {
            str = Regex.Replace(str, "[ії]", "i", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "[\\/:*?<>|]", "");
            return str;
        }

        public static bool IsEmpty(this string str) {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotEmpty(this string str) {
            return string.IsNullOrWhiteSpace(str) == false;
        }

        public static string ToCamel(this string str) {
            if (str.IsEmpty()) {
                return str;
            }

            var split = Regex.Replace(str, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

            split = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(split.ToLower());
            split = split.Replace(" ", string.Empty);
            return split;
        }

        public static string ToTitle(this string str) {
            return str.IsEmpty()
                ? str
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string F(this string str, params object[] args) {
            return string.Format(str, args);
        }

        public static string F(this string str, IFormatProvider provider, params object[] args) {
            return string.Format(provider, str, args);
        }

        public static bool Like(this string toSearch, string toFind) {
            var result =
                new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch)
                                                                                   .Replace('_', '.')
                                                                                   .Replace("%", ".*") + @"\z"
                    ,
                    RegexOptions.Singleline).IsMatch(toSearch);
            return result;
        }

        public static string FormatSymbols(this string str) {
            if (str.Length < 1) {
                return str;
            }
            return str[0] == '[' && str.Length > 2
                ? str.Substring(1, str.Length - 2)
                : str;
        }

        public static string FormatFieldFrom(this string str) {
            if (str == null) {
                return string.Empty;
            }
            return str.Replace("\n", "\\n")
                      .Replace("\r", "\\r")
                      .Replace(@"\", @"\\")
                      .Replace("'", "\\'");
        }

        public static string FormatFieldTo(this string str) {
            if (str == null) {
                return string.Empty;
            }
            return str.Replace(@"\\r", @"\r")
                      .Replace(@"\\n", @"\n");
        }

        public static SqlDbType ToDbType(this string propertyType) {
            switch (propertyType) {
                case "short":
                    return SqlDbType.SmallInt;
                case "byte":
                case "byte?":
                    return SqlDbType.TinyInt;
                case "int":
                case "int?":
                    return SqlDbType.Int;
                case "long":
                case "long?":
                    return SqlDbType.BigInt;
                case "string":
                    return SqlDbType.NVarChar;
                case "decimal":
                case "decimal?":
                    return SqlDbType.Decimal;
                case "double":
                case "double?":
                case "float":
                case "float?":
                    return SqlDbType.Float;
                case "datetime":
                case "datetime?":
                    return SqlDbType.DateTime2;
                case "bool":
                case "bool?":
                    return SqlDbType.Bit;
            }
            return SqlDbType.NVarChar;
        }

        public static object FromatField(this string source, string propertyType) {
            switch (propertyType) {
                case "short":
                    return Parse<short>(source);
                case "byte":
                    return Parse<byte>(source);
                case "byte?":
                    return Parse<byte?>(source);
                case "int":
                    return Parse<int>(source);
                case "int?":
                    return Parse<int?>(source);
                case "long":
                    return Parse<long>(source);
                case "long?":
                    return Parse<long?>(source);
                case "string":
                    return source;
                case "decimal":
                    return Parse<decimal>(source);
                case "decimal?":
                    return Parse<decimal?>(source);
                case "double":
                    return Parse<double>(source);
                case "double?":
                    return Parse<double?>(source);
                case "float":
                    return Parse<float>(source);
                case "float?":
                    return Parse<float?>(source);
                case "datetime":
                    return Parse<DateTime>(source);
                case "datetime?":
                    return Parse<DateTime?>(source);
                case "bool":
                    return Parse<bool>(source);
                case "bool?":
                    return Parse<bool?>(source);
            }
            return null;
        }

        public static bool ToBool(this string source) {
            return !source.IsEmpty() && Parse<bool>(source);
        }

        public static DateTime? ToDateTime(this string source) {
            return Parse<DateTime?>(source);
        }

        public static TimeSpan? ToTimeSpan(this string source) {
            return Parse<TimeSpan?>(source);
        }

        public static double ToDouble(this string source) {
            return Parse<double>(source);
        }

        public static int ToInt(this string source) {
            return Parse<int>(source);
        }

        public static long ToLong(this string source) {
            return Parse<long>(source);
        }

        public static string[] Split(this string source,
                                     string splitter,
                                     StringSplitOptions options = StringSplitOptions.None) {
            return source.IsEmpty()
                ? new string[] {}
                : source.Split(new[] {
                    splitter
                },
                    options);
        }

        public static string[] SplitRemoveEmpty(this string source, string splitter) {
            var result = new string[] {};

            if (source.IsNotEmpty()) {
                result = source.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            }

            return result;
        }

        public static string[] SplitRemoveEmpty(this string source, char splitter) {
            var result = new string[] {};

            if (source.IsNotEmpty()) {
                result = source.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            }

            return result;
        }


        public static List<T> SplitToList<T>(this string input,
                                             string splitter = ",",
                                             StringSplitOptions options = StringSplitOptions.None) {
            return SplitToList<T>(input,
                new[] {
                    splitter
                },
                options);
        }


        public static List<T> SplitToList<T>(this string input,
                                             string[] splitterArray,
                                             StringSplitOptions options) {
            var result = new List<T>();
            if (input.IsEmpty()) {
                return result;
            }

            var lines = input.Split(splitterArray, options);
            result.AddRange(lines.Select(Parse<T>));

            return result;
        }

        public static T Parse<T>(string text) {
            var type = typeof (T);
            var result = default(T);
            var input = text.Trim();

            //bool
            if (type == typeof (bool) || type == typeof (bool?)) {
                bool res;
                int intRes;
                if (bool.TryParse(input, out res)) {
                    return (T) Convert.ChangeType(res, typeof (bool));
                }
                if (int.TryParse(input, out intRes)) {
                    return (T) Convert.ChangeType(intRes != 0, typeof (bool));
                }
                return result;
            }

            //int
            if (type == typeof (int) || type == typeof (int?)) {
                int res;
                return int.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (int))
                    : result;
            }

            //uint
            if (type == typeof (uint) || type == typeof (uint?)) {
                uint res;
                return uint.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (uint))
                    : result;
            }

            //byte
            if (type == typeof (byte) || type == typeof (byte?)) {
                byte res;
                return byte.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (byte))
                    : result;
            }

            //decimal
            if (type == typeof (decimal) || type == typeof (decimal?)) {
                decimal res;
                return decimal.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (decimal))
                    : result;
            }

            //float
            if (type == typeof (float) || type == typeof (float?)) {
                var res = ParseDouble(text);
                return res == null
                    ? result
                    : (T) Convert.ChangeType((float) res, typeof (float));
            }

            //ulong
            if (type == typeof (ulong) || type == typeof (ulong?)) {
                ulong res;
                return ulong.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (ulong))
                    : result;
            }

            //short
            if (type == typeof (short) || type == typeof (short?)) {
                short res;
                return short.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (short))
                    : result;
            }

            //short
            if (type == typeof (ushort) || type == typeof (ushort?)) {
                ushort res;
                return ushort.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (ushort))
                    : result;
            }

            //long
            if (type == typeof (long) || type == typeof (long?)) {
                long res;
                return long.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (long))
                    : result;
            }

            //sbyte
            if (type == typeof (sbyte) || type == typeof (sbyte?)) {
                sbyte res;
                return sbyte.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (sbyte))
                    : result;
            }

            //single
            if (type == typeof (Single) || type == typeof (Single?)) {
                Single res;
                return Single.TryParse(input, out res)
                    ? (T) Convert.ChangeType(res, typeof (Single))
                    : result;
            }

            //double
            if (type == typeof (double) || type == typeof (double?)) {
                var res = ParseDouble(text);
                return res == null
                    ? result
                    : (T) Convert.ChangeType(res, typeof (double));
            }

            //datetime
            if (type == typeof (DateTime) || type == typeof (DateTime?)) {
                var res = ParseDateTime(text);
                if (res == null) {
                    return result;
                }
                if (res < _dateTimeMinValue) {
                    return result;
                }

                return (T) Convert.ChangeType(res, typeof (DateTime));
            }

            //timespan
            if (type == typeof (TimeSpan) || type == typeof (TimeSpan?)) {
                var res = ParseTimeSpan(text);
                return res == null
                    ? result
                    : (T) Convert.ChangeType(res, typeof (TimeSpan));
            }

            return result;
        }

        private static TimeSpan? ParseTimeSpan(this string input) {
            if (input.IsEmpty()) {
                return null;
            }
            DateTime result;
            TimeSpan res;
            if (DateTime.TryParse(input, out result)) {
                if (TimeSpan.TryParse(result.ToString("HH:mm:ss"), out res)) {
                    return res;
                }
                return null;
            }
            if (TimeSpan.TryParse(input, out res)) {
                return res;
            }
            return null;
        }

        private static DateTime? ParseDateTime(this string input) {
            DateTime? res;

            if (input.IsEmpty()) {
                return null;
            }

            var intValue = input.ToInt();
            if (input == intValue.ToString(CultureInfo.InvariantCulture)) {
                if (intValue > 200000) {
                    res = null;
                } else {
                    res = _dateTimeMinValue.AddDays(intValue - 2);
                }
            } else {
                DateTime d;
                var objCulture = Thread.CurrentThread.CurrentCulture;

                Thread.CurrentThread.CurrentCulture = !input.Contains("/")
                    ? new CultureInfo("ru-RU")
                    : new CultureInfo("en-US");

                if (DateTime.TryParse(input, out d)) {
                    res = d;
                } else {
                    res = null;
                }

                Thread.CurrentThread.CurrentCulture = objCulture;
            }
            if (res == null) {
                return null;
            }

            return res >= _dateTimeMinValue
                ? res
                : null;
        }

        private static double? ParseDouble(this string input) {
            if (input.IsEmpty()) {
                return null;
            }

            var sep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            if (input.Contains(sep)) {
                input = input.Replace(sep == "."
                    ? ","
                    : ".",
                    string.Empty);
            } else {
                input = input.Replace(",", sep)
                             .Replace(".", sep);
            }
            if ((!Regex.IsMatch(input, "^-?[0-9]+[+E0-9]*$") &&
                 !Regex.IsMatch(input, "^-?[0-9]+[" + sep + "]?[0-9]+[+E0-9]*$")) || Equals(input, sep)) {
                return null;
            }
            double res;
            return Double.TryParse(input, out res)
                ? (double?) res
                : null;
        }
    }
}