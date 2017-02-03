using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Utility.Collections
{
	internal sealed class HybridDictionaryValueCollectionDebugView<TKey, TValue>
	{
		private readonly ICollection<TValue> _collection;

		public HybridDictionaryValueCollectionDebugView(ICollection<TValue> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			_collection = collection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public TValue[] Items
		{
			get
			{
				var items = new TValue[_collection.Count];
				_collection.CopyTo(items, 0);
				return items;
			}
		}
	}
}