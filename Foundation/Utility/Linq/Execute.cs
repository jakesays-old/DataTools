using System;
using System.Collections.Generic;
using System.Linq;

namespace Std.Utility.Linq
{
	static partial class MoreEnumerable
	{
		public static IEnumerable<TItem> Execute<TItem>(this IQueryable<TItem> query)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			return query;
		}
	}
}