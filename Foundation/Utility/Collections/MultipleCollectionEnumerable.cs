using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Std.Utility.Collections
{
	public class MultipleCollectionEnumerable<TElementType> : IEnumerable<TElementType>
	{
		private readonly IEnumerable<TElementType>[] _collections;

		public MultipleCollectionEnumerable(params IEnumerable<TElementType>[] collections)
		{
			_collections = collections;
		}

		public IEnumerator<TElementType> GetEnumerator()
		{
			return _collections.SelectMany(collection => collection).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}