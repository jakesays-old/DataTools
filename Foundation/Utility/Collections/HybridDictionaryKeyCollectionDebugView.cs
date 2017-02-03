using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Utility.Collections
{
	internal sealed class HybridDictionaryKeyCollectionDebugView<TKey, TValue>
	{
		private readonly ICollection<TKey> _collection;

		public HybridDictionaryKeyCollectionDebugView(ICollection<TKey> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			_collection = collection;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public TKey[] Items
		{
			get
			{
				var items = new TKey[_collection.Count];
				_collection.CopyTo(items, 0);
				return items;
			}
		}
	}
}