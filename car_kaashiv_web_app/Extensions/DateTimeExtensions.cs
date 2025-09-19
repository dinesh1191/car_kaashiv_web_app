using System;

namespace car_kaashiv_web_app.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo istZone =
            TimeZoneInfo.FindSystemTimeZoneById(
#if WINDOWS
                "India Standard Time"
#else
                "Asia/Kolkata"
#endif
            );

        /// <summary>
        /// Converts a UTC DateTime to IST (India Standard Time).
        /// </summary>
        /// <param name="utcDateTime">UTC DateTime value</param>
        /// <returns>DateTime in IST</returns>
        public static DateTime ToIST(this DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istZone);
        }
    }
}
