using System;
using System.Diagnostics;

namespace Std.Utility.Internal
{
	public class DefaultTimeProvider : TimeProvider
	{
		public override DateTime Now
		{
			[DebuggerStepThrough]
			get { return DateTime.Now; }
		}

		public override DateTime UtcNow
		{
			[DebuggerStepThrough]
			get { return DateTime.UtcNow; }
		}
	}
}