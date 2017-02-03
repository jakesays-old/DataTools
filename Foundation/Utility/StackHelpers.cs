using System;
using System.Diagnostics;

namespace Std.Utility
{
	/// <summary>
	/// Provides methods for inspecting the call stack.
	/// </summary>
	public static class StackHelpers
	{
		/// <summary>
		/// Walks the call stack looking for a particular method. Used to verify
		/// a method is being used properly.
		/// </summary>
		/// <param name="targetType">The type that <paramref name="targetMethod"/>is a member of.</param>
		/// <param name="targetMethod">Name of the method to search for.
		/// Use <c>.ctor</c> for an instance constructor and
		/// <c>.cctor</c>for a static constructor</param>
		/// <param name="maximumFrameCount">Maximum number of stack frames to check.
		/// The default is the smaller of eight frames or the height of the stack.</param>
		/// <returns><c>true</c>if <paramref name="targetMethod"/>is found within
		/// <paramref name="maximumFrameCount"/>stack frames, <c>false</c>otherwise.</returns>
		public static bool IsCallerPresent(Type targetType, string targetMethod, int maximumFrameCount = 8)
		{
			var stack = new StackTrace();
			maximumFrameCount = Math.Min(maximumFrameCount, stack.FrameCount);

			for (var frameIndex = 0; frameIndex < maximumFrameCount; frameIndex++)
			{
				var caller = stack.GetFrame(frameIndex).GetMethod();
				if (caller.Name == targetMethod &&
					caller.DeclaringType == targetType)
				{
					return true;
				}
			}

			return false;
		}		
	}
}