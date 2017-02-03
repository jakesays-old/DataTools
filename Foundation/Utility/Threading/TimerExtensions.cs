using System;
using System.Threading;

namespace Std.Utility.Threading
{
	public static class TimerExtensions
	{
		public static Timer CreateStoppedTimer(TimerCallback handler, object state = null)
		{
			var timer = new Timer(handler, state, Timeout.Infinite, Timeout.Infinite);
			return timer;
		}

		public static bool Stop(this Timer timer)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}

			var result = timer.Change(Timeout.Infinite, Timeout.Infinite);
			return result;
		}

		public static bool Once(this Timer timer, int delay)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(delay, Timeout.Infinite);
			return result;
		}

		public static bool Repeat(this Timer timer, int period)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(period, period);
			return result;
		}

		public static bool Start(this Timer timer, int delay, int period)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(delay, period);
			return result;
		}

		public static bool Once(this Timer timer, TimeSpan delay)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(delay, Timeout.InfiniteTimeSpan);
			return result;
		}

		public static bool Repeat(this Timer timer, TimeSpan period)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(period, period);
			return result;
		}

		public static bool Start(this Timer timer, TimeSpan delay, TimeSpan period)
		{
			if (timer == null)
			{
				throw new ArgumentNullException("timer");
			}


			var result = timer.Change(delay, period);
			return result;
		}
	}
}