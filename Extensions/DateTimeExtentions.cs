using System;

namespace Extensions {
    public static class DateTimeExtentions {
        public static int MinutesOnDay(this DateTime? time) {
            return time == null
                       ? 0
                       : MinutesOnDay(time.Value);
        }

        public static int MinutesOnDay(this DateTime time) {
            return (time.Hour*60) + time.Minute;
        }

        public static DateTime MaxTimeOnDay(this DateTime time) {
            return time.Date.AddDays(1).AddSeconds(-1);
        }
    }
}