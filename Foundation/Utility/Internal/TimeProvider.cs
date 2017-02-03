using System;

namespace Std.Utility.Internal
{
	public abstract class TimeProvider
	{
		public static TimeProvider CurrentProvider = new DefaultTimeProvider();

		public abstract DateTime Now { get; }
		public abstract DateTime UtcNow { get; }
	}
}