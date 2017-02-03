using System;

namespace Std.Utility
{
	public static class ArrayExtensions
	{
		public static bool IsEmpty<TElement>(this TElement[] array)
		{
			return array == null || array.Length == 0;
		}

		public static bool NotEmpty<TElement>(this TElement[] array)
		{
			return array != null && array.Length > 0;
		}

		/// <summary>
		/// Returns the length of an array
		/// </summary>
		/// <typeparam name="TElement">The array element type</typeparam>
		/// <param name="array">The array in question</param>
		/// <returns>If <paramref name="array"/> is <c>null</c> then 0, otherwise array.Length</returns>
		public static int SafeLength<TElement>(this TElement[] array)
		{
			if (array == null)
			{
				return 0;
			}

			return array.Length;
		}
	}
}