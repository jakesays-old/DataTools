using System.Collections.Generic;
using Std.Utility.Collections;

namespace Std.Utility
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Performs safe enumeration over an enumerable
		/// by ensuring the enumerable is never null.
		/// </summary>
		/// <typeparam name="TItem">Type of the enumerable.</typeparam>
		/// <param name="container">Container to enumerate over.</param>
		/// <returns>Container if container != null, an empty TItem[] otherwise.</returns>
		public static IEnumerable<TItem> Enumerate<TItem>(this IEnumerable<TItem> container)
		{
			if (container == null)
			{
				return Empty.Array<TItem>();
			}

			return container;
		}
	}
}