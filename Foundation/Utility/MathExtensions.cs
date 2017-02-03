using System;

namespace Std.Utility
{
	public static class MathExtensions
	{
		/// <summary>
		/// float extension that does equality comparison using an epsilon value.
		/// </summary>
		/// <param name="number">The target float.</param>
		/// <param name="rhs">The float to compare against.</param>
		/// <param name="epsilon">Epsilon used to determine if the float values are equal.</param>
		/// <returns>true if the absolute value of the difference between number and rhs is less than
		/// epsilon, false otherwise.</returns>
		public static bool EqualsEpsilon(this float number, float rhs, float epsilon)
		{
			return Math.Abs(number - rhs) < epsilon;
		}

		/// <summary>
		/// float extension that does equality comparison using an epsilon value.
		/// </summary>
		/// <param name="number">The target float.</param>
		/// <param name="rhs">The float to compare against.</param>
		/// <returns>true if the absolute value of the difference between number and rhs is less than
		/// Single.Epsilon, false otherwise.</returns>
		public static bool EqualsEpsilon(this float number, float rhs)
		{
			return EqualsEpsilon(number, rhs, float.Epsilon);
		}
	}
}