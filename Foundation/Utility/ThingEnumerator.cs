using System.Collections.Generic;

namespace Std.Utility
{
	/// <summary>
	/// Enumerates over a list of things.
	/// </summary>
	public static class ThingEnumerator
	{
		public static IEnumerable<TThing> Over<TThing>(params TThing[] things)
		{
			return things;
		}
	}
}