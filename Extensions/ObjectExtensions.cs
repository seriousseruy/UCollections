using System;

namespace Extensions {
    public static class ObjectExtensions {
        public static int ToInt(this object source) {
            if (source == null || source == DBNull.Value) {
                return 0;
            }

            if (source is int) {
                return (int) source;
            }

            if (source is Enum) {
                return source.GetHashCode();
            }

            if (source is double) {
                return (int) Math.Round((double) source);
            }

            if (source is bool) {
                return (bool) source
                           ? 1
                           : 0;
            }

            return source.ToString()
                         .ToInt();
        }

        public static long ToLong(this object source) {
            if (source == null || source == DBNull.Value) {
                return 0;
            }

            if (source is long) {
                return (long) source;
            }

            if (source is bool) {
                return (bool) source
                           ? 1
                           : 0;
            }

            return source.ToString()
                         .ToLong();
        }

        public static double ToDouble(this object source) {
            if (source == null || source == DBNull.Value) {
                return 0;
            }

            if (source is double) {
                return (double) source;
            }

            return source.ToString()
                         .ToDouble();
        }

        public static DateTime? ToDateTime(this object source) {
            if (source == null || source == DBNull.Value) {
                return null;
            }

            if (source is DateTime) {
                return (DateTime) source;
            }

            return source.ToString()
                         .ToDateTime();
        }

        public static TimeSpan? ToTimeSpan(this object source) {
            if (source == null || source == DBNull.Value) {
                return null;
            }

            if (source is TimeSpan) {
                return (TimeSpan) source;
            }

            return source.ToString()
                         .ToTimeSpan();
        }

        public static string ToString(this object source) {
            return source == null || source == DBNull.Value
                       ? string.Empty
                       : source.ToString();
        }

        public static bool ToBool(this object source) {
            if (source == null || source == DBNull.Value) {
                return false;
            }

            if (source is bool) {
                return (bool) source;
            }

            if (source is string) {
                return source.ToString()
                             .ToBool();
            }

            if (source is int) {
                return (int) source != 0;
            }

            if (source is decimal) {
                return (decimal) source != 0;
            }

            return false;
        }

        public static T Parse<T>(this object source) {
            var type = typeof (T);
            var result = default(T);
            if (source == null) {
                return result;
            }

            //bool
            if (type == typeof (bool) || type == typeof (bool?)) {
                return (T) Convert.ChangeType(source.ToBool(), typeof (bool));
            }

            //int
            if (type == typeof (int) || type == typeof (int?)) {
                return (T) Convert.ChangeType(source.ToInt(), typeof (int));
            }

            //long
            if (type == typeof (long) || type == typeof (long?)) {
                return (T) Convert.ChangeType(source.ToLong(), typeof (long));
            }

            //Ulong
            if (type == typeof (ulong)) {
                return (T) Convert.ChangeType(source.ToLong(), typeof (T));
            }

            //double
            if (type == typeof (double) || type == typeof (double?)) {
                return (T) Convert.ChangeType(source.ToDouble(), typeof (double));
            }

            //datetime
            if (type == typeof (DateTime) || type == typeof (DateTime?)) {
                var date = source.ToDateTime();
                return date == null
                           ? default(T)
                           : (T) Convert.ChangeType(date.Value, typeof (DateTime));
            }

            //guid
            if (type == typeof (Guid)) {
                return source is Guid
                           ? (T) Convert.ChangeType((Guid) source, typeof (Guid))
                           : default(T);
            }

            //string
            if (type == typeof (string)) {
                return (T) Convert.ChangeType(source.ToString(), typeof (T));
            }

            return result;
        }
    }
}