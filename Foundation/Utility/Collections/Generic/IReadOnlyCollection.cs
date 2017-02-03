using System.Collections;
using System.Collections.Generic;

namespace Std.Utility.Collections.Generic
{
	public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
	{
		int Count
		{
			get;
		}
	}
}