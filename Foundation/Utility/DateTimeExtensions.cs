using System;
using JetBrains.Annotations;

namespace Std.Utility
{
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether value represents a leap day
		/// </summary>
		/// <param name="value">The date in question</param>
		/// <returns>True if value is a leap year and the date is 2/29.</returns>
		public static bool IsLeapDay(this DateTime value)
		{
			if (!DateTime.IsLeapYear(value.Year))
			{
				return false;
			}

			return value.Month == 2 && value.Day == 29;
		}

		public static DateTime SetTimeOfDay(this DateTime dateTime, TimeSpan newTimeOfDay)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, newTimeOfDay.Hours, newTimeOfDay.Minutes,
				newTimeOfDay.Seconds, newTimeOfDay.Milliseconds);
		}

		public static bool IsLastDayOfTheMonth(this DateTime dateTime)
		{
			return (dateTime.Day == dateTime.DaysInMonth());
		}

		public static int DaysInMonth(this DateTime dateTime)
		{
			return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
		}

		/// <summary>
		/// Determines whether <paramref name="now"/> is equal to <c>DateTime.MinValue</c>.
		/// </summary>
		/// <param name="now">The value to check.</param>
		/// <returns>
		/// 	<c>true</c> if <paramref name="now"/> == <c>DateTime.MinValue</c>; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsMinValue(this DateTime now)
		{
			return now == DateTime.MinValue;
		}

		/// <summary>
		/// Determines if <paramref name="now"/> has been set to a real value.
		/// </summary>
		/// <param name="now">The value in question.</param>
		/// <returns><c>true</c>if <paramref name="now"/> is neither <c>DateTime.MinValue</c>
		/// nor <c>DateTime.MaxValue</c>.</returns>
		public static bool HasValue(this DateTime now)
		{
			return now != DateTime.MinValue && now != DateTime.MaxValue;
		}

		[ContractAnnotation("now:null => false")]
		public static bool NotNullOrEmpty(this DateTime? now)
		{
			return now.HasValue && HasValue(now.Value);
		}

		[ContractAnnotation("now:null => true")]
		public static bool IsNullOrEmpty(this DateTime? now)
		{
			return now == null || !HasValue(now.Value);
		}

		/// <summary>
		/// Clips <paramref name="now"/> to the earliest time value of its month.
		/// </summary>
		/// <param name="now">The value to clip</param>
		/// <returns><paramref name="now"/> as "MM/01/YYYY 00:00:00.000</returns>
		public static DateTime ClipToStartOfMonth(this DateTime now)
		{
			return new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0);
		}

		/// <summary>
		/// Clips <paramref name="now"/> to the latest time value of its month.
		/// </summary>
		/// <param name="now">The value to clip</param>
		/// <returns><paramref name="now"/> as "MM/DD/YYYY 23:59:59.999 where DD is the last day of the month.</returns>
		public static DateTime ClipToEndOfMonth(this DateTime now)
		{
			return new DateTime(now.Year, now.Month, now.DaysInMonth(), 23, 59, 59, 999);
		}

		/// <summary>
		/// Determines whether <paramref name="now"/> falls within a closed range.
		/// Both the left and right limits are included in the comparison
		/// </summary>
		/// <param name="now">The value to compare.</param>
		/// <param name="leftLimit">Left side of range</param>
		/// <param name="rightLimit">Right side of range</param>
		/// <returns><c>true</c> if <paramref name="now"/> is between <paramref name="leftLimit"/>
		/// and <paramref name="rightLimit"/></returns>
		public static bool IsInClosedRange(this DateTime now, DateTime leftLimit, DateTime rightLimit)
		{
			return (now >= leftLimit) && (now <= rightLimit);
		}

		/// <summary>
		/// Determines whether <paramref name="now"/> is within
		/// <paramref name="minutes"/> of <paramref name="target"/>.
		/// </summary>
		/// <param name="now">The time value in question.</param>
		/// <param name="target">The time value to compare against</param>
		/// <param name="minutes">Defines the range in minutes around the target time.</param>
		/// <returns><c>true</c>if <paramref name="now"/>is between <paramref name="target"/>
		/// -<paramref name="minutes"/> and <paramref name="target"/>+<paramref name="minutes"/>.</returns>
		public static bool WithinMinutesOf(this DateTime now, DateTime target, int minutes)
		{
			return (now > target.AddMinutes(-minutes)) && (now < target.AddMinutes(minutes));
		}

		/// <summary>
		/// Moves <paramref name="now"/> to the beginning of the day, which is
		/// 00:00:00
		/// </summary>
		/// <param name="now">Value to be adjusted.</param>
		/// <returns>New <c>DateTime</c>equal to <paramref name="now"/>.Date + 23:59:59.999</returns>
		public static DateTime StartOfDay(this DateTime now)
		{
			return now.Date;
		}

		/// <summary>
		/// Moves <paramref name="now"/> to the end of the day, which is
		/// 00:00:00 - 1 ms
		/// </summary>
		/// <param name="now">Value to be adjusted.</param>
		/// <returns>New <c>DateTime</c>equal to <paramref name="now"/>.Date + 23:59:59.999</returns>
		public static DateTime EndOfDay(this DateTime now)
		{
			return new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999);
		}

		/// <summary>
		/// Similar to DateTime.ToString("s"), but more readable.
		/// </summary>
		/// <param name="now">The <c>DateTime</c> to format.</param>
		/// <returns><paramref name="now"/>formatted as 2010-03-18 00:38:00.567</returns>
		public static string ToSortableString(this DateTime now)
		{
			return now.ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		/// <summary>
		/// DateTime.ToString that is a little more readable.
		/// </summary>
		/// <param name="now">The <c>DateTime</c> to format.</param>
		/// <returns><paramref name="now"/>formatted as 03-18-2010 00:38:00</returns>
		public static string ToDisplayString(this DateTime now)
		{
			return now.ToString("MM-dd-yyyy HH:mm:ss");
		}

		/// <summary>
		/// Returns the date formatted as a sortable string suitable
		/// for use in a file name.
		/// </summary>
		/// <param name="now">The <c>DateTime</c> to format.</param>
		/// <returns><paramref name="now"/>formatted as 20100318T003800567</returns>
		public static string ToFileNameString(this DateTime now)
		{
			return now.ToString("yyyyMMdd") + "T" + now.ToString("HHmmssfff");
		}

		public static DateTime Min(DateTime arg0, DateTime arg1)
		{
			if (arg0 <= arg1)
			{
				return arg0;
			}

			return arg1;
		}

		public static DateTime Max(DateTime arg0, DateTime arg1)
		{
			if (arg0 >= arg1)
			{
				return arg0;
			}

			return arg1;
		}

		//very handy methods lifted from ServiceStack
		public const long UnixEpoch = 621355968000000000L;
		private static readonly DateTime UnixEpochDateTime = new DateTime(UnixEpoch);

		public const long TicksPerMs = TimeSpan.TicksPerSecond / 1000;

		public static long ToUnixTime(this DateTime dateTime)
		{
			var epoch = (dateTime.ToUniversalTime().Ticks - UnixEpoch) / TimeSpan.TicksPerSecond;
			return epoch;
		}

		public static DateTime FromUnixTime(this double unixTime)
		{
			return UnixEpochDateTime + TimeSpan.FromSeconds(unixTime);
		}

		public static long ToUnixTimeMs(this DateTime dateTime)
		{
			var epoch = (dateTime.ToUniversalTime().Ticks - UnixEpoch) / TicksPerMs;
			return epoch;
		}

		public static DateTime FromUnixTimeMs(this double msSince1970)
		{
			var ticks = (long) (UnixEpoch + (msSince1970 * TicksPerMs));
			return new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
		}

		public static DateTime RoundToSecond(this DateTime dateTime)
		{
			return new DateTime(((dateTime.Ticks) / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
		}

		public static bool IsEqualToTheSecond(this DateTime dateTime, DateTime otherDateTime)
		{
			return dateTime.ToUniversalTime().RoundToSecond().Equals(otherDateTime.ToUniversalTime().RoundToSecond());
		}
	}
}
