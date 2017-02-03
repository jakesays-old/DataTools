using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Utility.Collections
{
	internal sealed class HybridDictionaryDebugView<K, V>
	{
		private readonly IDictionary<K, V> _dict;

		public HybridDictionaryDebugView(IDictionary<K, V> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			_dict = dictionary;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<K, V>[] Items
		{
			get
			{
				var items = new KeyValuePair<K, V>[_dict.Count];
				_dict.CopyTo(items, 0);
				return items;
			}
		}
	}
}