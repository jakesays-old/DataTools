using System;
using System.Diagnostics;
using Std.Utility.Internal;

namespace Std.Utility
{
	/// <summary>
	/// The SystemTime class provides a simple abstraction over DateTime.Now.
	/// It uses a time provider to generate the current time. The default
	/// provider just fowards to DateTime.Now/UtcNow.
	/// </summary>
	public static class SystemTime
	{
		public static DateTime Now
		{
			[DebuggerStepThrough]
			get { return TimeProvider.CurrentProvider.Now; }
		}

		public static DateTime UtcNow
		{
			[DebuggerStepThrough]
			get { return TimeProvider.CurrentProvider.UtcNow; }
		}
	}
}