using System;
using System.Collections;
using System.Collections.Generic;

namespace Std.Utility.Collections.Generic
{
	/// <summary>
	///     An implementation of a multiple value Dictionary; see
	///     <seealso cref="T:System.Collections.Generic.IMultiDictionary`2" />
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class MultiDictionary<TKey, TValue> : IMultiDictionary<TKey, TValue>,
		ICollection<KeyValuePair<TKey, TValue>>,
		IEnumerable<KeyValuePair<TKey, TValue>>,
		IReadOnlyDictionary<TKey, ICollection<TValue>>,
		IReadOnlyCollection<KeyValuePair<TKey, ICollection<TValue>>>,
		IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>,
		IEnumerable
	{
		/// <summary>
		///     The private dictionary that this class effectively wraps around
		/// </summary>
		private readonly Dictionary<TKey, ICollection<TValue>> _dictionary;

		/// <summary>
		///     The number of key-value pairs currently in the MultiDictionary
		/// </summary>
		private int _count;

		/// <summary>
		///     The current version of this MultiDictionary used to determine MultiDictionary modification
		///     during enumeration
		/// </summary>
		private int _version;

		/// <summary>
		///     A ValueCollection to return from the Values property
		/// </summary>
		private ValueCollection _values;

		/// <summary>
		///     The number of KeyValuePairs in this MultiDictionary
		/// </summary>
		public int Count
		{
			get { return _count; }
		}

		/// <summary>
		///     True if the MultiDictionary is read only, false otherwise.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		///     Get all values associated with the given key. If there are not
		///     currently any values associated with the key, then an empty
		///     <see cref="T:System.Collections.Generic.ICollection`1" />
		///     is created for that key and returned.
		/// </summary>
		/// <param name="key">The key to get all values for</param>
		/// <returns>A collection of values that are associated with the given key</returns>
		/// <remarks>
		///     Note that the collection returned will change alongside any changes to the multiDictionary (and vice-versa)
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public ICollection<TValue> this[TKey key]
		{
			get
			{
//				Requires.NotNullAllowStructs<TKey>(key, "key");
				return new InnerCollectionView(this, key);
			}
		}

		/// <summary>
		///     Gets a collection of all of the individual keys. Will only return keys that
		///     have one or more associated values.
		/// </summary>
		public ICollection<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}

		int IReadOnlyCollection<KeyValuePair<TKey, ICollection<TValue>>>.Count
		{
			get { return _dictionary.Count; }
		}

		IEnumerable<TKey> IReadOnlyDictionary<TKey, ICollection<TValue>>.Keys
		{
			get { return Keys; }
		}

		IEnumerable<ICollection<TValue>> IReadOnlyDictionary<TKey, ICollection<TValue>>.Values
		{
			get { return _dictionary.Values; }
		}

		/// <summary>
		///     Gets a collection of all of the individual values in this MultiDictionary
		/// </summary>
		public ICollection<TValue> Values
		{
			get
			{
				if (_values == null)
				{
					_values = new ValueCollection(this);
				}
				return _values;
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class
		///     that is empty, has the default initial capacity, and uses the default
		///     <see cref="T:System.Collections.Generic.IEqualityComparer`1" />. for the key type
		/// </summary>
		public MultiDictionary()
		{
			_dictionary = new Dictionary<TKey, ICollection<TValue>>();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class that is
		///     empty, has the specified initial capacity, and uses the default
		///     <see cref="T:System.Collections.Generic.IEqualityComparer`1" />. for the key type.
		/// </summary>
		/// <param name="capacity">Initial number of keys that the MultiDictionary will allocate space for</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Capacity must be &gt;= 0</exception>
		public MultiDictionary(int capacity)
		{
//			Requires.Range(capacity >= 0, "capacity", null);
			_dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class
		///     that is empty, has the default initial capacity, and uses the
		///     specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
		/// </summary>
		/// <param name="comparer">Specified comparer to use for the keys</param>
		public MultiDictionary(IEqualityComparer<TKey> comparer)
		{
			_dictionary = new Dictionary<TKey, ICollection<TValue>>(comparer);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class
		///     that is empty, has the specified initial capacity, and uses the
		///     specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.
		/// </summary>
		/// <param name="capacity">Initial number of keys that the MultiDictionary will allocate space for</param>
		/// <param name="comparer">Specified comparer to use for the keys</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Capacity must be &gt;= 0</exception>
		public MultiDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
//			Requires.Range(capacity >= 0, "capacity", null);
			_dictionary = new Dictionary<TKey, ICollection<TValue>>(capacity, comparer);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class that contains
		///     elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; and uses the
		///     default <see cref="T:System.Collections.Generic.IEqualityComparer`1" />. for the key type.
		/// </summary>
		/// <param name="enumerable">IEnumerable to copy elements into this from</param>
		/// <exception cref="T:System.ArgumentNullException">enumerable must be non-null</exception>
		public MultiDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
			: this(enumerable, null)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Collections.Generic.MultiDictionary`2" /> class that contains
		///     elements copied from the specified IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; and uses the
		///     specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />..
		/// </summary>
		/// <param name="enumerable">IEnumerable to copy elements into this from</param>
		/// <param name="comparer">Specified comparer to use for the keys</param>
		/// <exception cref="T:System.ArgumentNullException">enumerable must be non-null</exception>
		public MultiDictionary(IEnumerable<KeyValuePair<TKey, TValue>> enumerable, IEqualityComparer<TKey> comparer)
		{
//			Requires.NotNullAllowStructs<IEnumerable<KeyValuePair<TKey, TValue>>>(enumerable, "enumerable");
			_dictionary = new Dictionary<TKey, ICollection<TValue>>(comparer);
			foreach (var keyValuePair in enumerable)
			{
				Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		/// <summary>
		///     Adds the specified key and value to the MultiDictionary.
		/// </summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add.</param>
		/// <remarks>
		///     Unlike the Add for <see cref="T:System.Collections.IDictionary" />, the MultiDictionary Add will not
		///     throw any exceptions. If the given key is already in the MultiDictionary,
		///     then value will be added to that keys associated values collection.
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public void Add(TKey key, TValue value)
		{
			ICollection<TValue> tValues;
//			Requires.NotNullAllowStructs<TKey>(key, "key");
			if (!_dictionary.TryGetValue(key, out tValues))
			{
				tValues = NewCollection(null);
				tValues.Add(value);
				_dictionary.Add(key, tValues);
			}
			else
			{
				tValues.Add(value);
			}
			var multiDictionary = this;
			multiDictionary._count = multiDictionary._count + 1;
			var multiDictionary1 = this;
			multiDictionary1._version = multiDictionary1._version + 1;
		}

		/// <summary>
		///     Adds the specified KeyValuePair to the MultiDictionary.
		/// </summary>
		/// <param name="item">KeyValuePair to add to this MultiDictionary.</param>
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		/// <summary>
		///     Adds a number of key-value pairs to this MultiDictionary, where
		///     the key for each value is the key param, and the value for a pair
		///     is an element from "values"
		/// </summary>
		/// <param name="key">The key of all entries to add</param>
		/// <param name="values">An IEnumerable of values to add</param>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		/// <exception cref="T:System.ArgumentNullException">values must be non-null</exception>
		public void AddRange(TKey key, IEnumerable<TValue> values)
		{
			ICollection<TValue> tValues;
			//Requires.NotNullAllowStructs<TKey>(key, "key");
			//Requires.NotNullAllowStructs<IEnumerable<TValue>>(values, "values");
			if (!_dictionary.TryGetValue(key, out tValues))
			{
				tValues = NewCollection(values);
				_dictionary.Add(key, tValues);
				var count = this;
				count._count = count._count + tValues.Count;
			}
			else
			{
				foreach (var value in values)
				{
					tValues.Add(value);
					var multiDictionary = this;
					multiDictionary._count = multiDictionary._count + 1;
				}
			}
			var multiDictionary1 = this;
			multiDictionary1._version = multiDictionary1._version + 1;
		}

		/// <summary>
		///     Removes all keys and values from this MultiDictionary
		/// </summary>
		public void Clear()
		{
			_count = 0;
			var multiDictionary = this;
			multiDictionary._version = multiDictionary._version + 1;
			_dictionary.Clear();
		}

		/// <summary>
		///     Determines if the given key-value pair exists within the MultiDictionary
		/// </summary>
		/// <param name="key">The key to check for</param>
		/// <param name="value">The value to check for</param>
		/// <returns>True if the MultiDictionary contains the requested pair, false otherwise</returns>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public bool Contains(TKey key, TValue value)
		{
			ICollection<TValue> tValues;
//			Requires.NotNullAllowStructs<TKey>(key, "key");
			if (!_dictionary.TryGetValue(key, out tValues))
			{
				return false;
			}
			return tValues.Contains(value);
		}

		/// <summary>
		///     Determines if the given KeyValuePair exists in this MultiDictionary
		/// </summary>
		/// <param name="item">KeyValuePair to search the MultiDictionary for</param>
		/// <returns>True if the pair is in this MultiDictionary, false otherwise</returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Contains(item.Key, item.Value);
		}

		/// <summary>
		///     Determines if the given key exists within this MultiDictionary and has
		///     at least one value associated with it.
		/// </summary>
		/// <param name="key">The key to search the MultiDictionary for</param>
		/// <returns>True if the MultiDictionary contains the requested key, false otherwise</returns>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public bool ContainsKey(TKey key)
		{
			ICollection<TValue> tValues;
//			Requires.NotNullAllowStructs<TKey>(key, "key");
			if (!_dictionary.TryGetValue(key, out tValues))
			{
				return false;
			}
			return tValues.Count > 0;
		}

		/// <summary>
		///     Determines if the given value exists within the MultiDictionary
		/// </summary>
		/// <param name="value">A value to search the MultiDictionary for</param>
		/// <returns>True if the MultiDictionary contains the requested value, false otherwise</returns>
		public bool ContainsValue(TValue value)
		{
			var enumerator = _dictionary.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null ||
						!enumerator.Current.Contains(value))
					{
						continue;
					}
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable) enumerator).Dispose();
			}
		}

		/// <summary>
		///     Copies all of the KeyValuePair items in this MultiDictionary into <paramref name="array" />
		///     starting at <paramref name="arrayIndex" />
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			//Requires.NotNullAllowStructs<KeyValuePair<TKey, TValue>[]>(array, "array");
			//Requires.Range(arrayIndex >= 0, "arrayIndex", null);
			//Requires.Range(arrayIndex <= (int) array.Length, "arrayIndex", null);
			//Requires.Argument((int) array.Length - arrayIndex >= this.Count, "arrayIndex", SR.GetString(Exceptions.CopyTo_ArgumentsTooSmall));
			foreach (var keyValuePair in this)
			{
				var index = arrayIndex;
				arrayIndex = index + 1;
				array[index] = keyValuePair;
			}
		}

		/// <summary>
		///     Get an <see cref="T:System.Collections.IEnumerator" /> that enumerates over
		///     <see cref="T:System.Collections.Generic.KeyValuePair`2" />
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		///     Protected Factory Method to create a new, empty instance of the ICollection that
		///     this MultiDictionary uses in its internal Dictionary
		/// </summary>
		protected virtual ICollection<TValue> NewCollection(IEnumerable<TValue> collection = null)
		{
			if (collection == null)
			{
				return new List<TValue>();
			}
			return new List<TValue>(collection);
		}

		/// <summary>
		///     Removes all values associated with the given key from the MultiDictionary
		/// </summary>
		/// <param name="key">The key of the items to be removed</param>
		/// <returns>True if the removal was successful, false otherwise</returns>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public bool Remove(TKey key)
		{
			ICollection<TValue> tValues;
//			Requires.NotNullAllowStructs<TKey>(key, "key");
			if (!_dictionary.TryGetValue(key, out tValues) || !_dictionary.Remove(key))
			{
				return false;
			}
			var count = this;
			count._count = count._count - tValues.Count;
			var multiDictionary = this;
			multiDictionary._version = multiDictionary._version + 1;
			return true;
		}

		/// <summary>
		///     Removes the first instance (if any) of the given KeyValuePair from the MultiDictionary.
		///     If the item being removed is the last one associated with its key, that key will be removed
		///     from the MultiDictionary and its associated values collection will be freed as if a call to Remove(item.key)
		///     had been made.
		/// </summary>
		/// <param name="item">The KeyValuePair to remove from this MultiDictionary</param>
		/// <returns>True if the removal was successful, false otherwise</returns>
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return RemoveItem(item.Key, item.Value);
		}

		/// <summary>
		///     Removes the first instance (if any) of the given key-value pair from the MultiDictionary.
		///     If the item being removed is the last one associated with its key, that key will be removed
		///     from the dictionary and its associated values collection will be freed as if a call to Remove(key)
		///     had been made.
		/// </summary>
		/// <param name="key">The key of the item to remove</param>
		/// <param name="value">The value of the item to remove</param>
		/// <returns>True if the removal was successful, false otherwise</returns>
		/// <exception cref="T:System.ArgumentNullException">key must be non-null</exception>
		public bool RemoveItem(TKey key, TValue value)
		{
			ICollection<TValue> tValues;
//			Requires.NotNullAllowStructs<TKey>(key, "key");
			if (!_dictionary.TryGetValue(key, out tValues) || !tValues.Remove(value))
			{
				return false;
			}
			if (tValues.Count == 0)
			{
				_dictionary.Remove(key);
			}
			var multiDictionary = this;
			multiDictionary._count = multiDictionary._count - 1;
			var multiDictionary1 = this;
			multiDictionary1._version = multiDictionary1._version + 1;
			return true;
		}

		IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> IEnumerable<KeyValuePair<TKey, ICollection<TValue>>>.
			GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		bool IReadOnlyDictionary<TKey, ICollection<TValue>>.TryGetValue(TKey key, out ICollection<TValue> value)
		{
			value = new InnerCollectionView(this, key);
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///     Returns an <see cref="T:System.Collections.IDictionary">IDictionary&lt;TKey,ICollection&lt;TValue&gt;t&gt;</see>
		///     that is a copy of the one
		///     inside of this MultiDictionary. Changes to the returned
		///     <see cref="T:System.Collections.IDictionary">IDictionary&lt;TKey,ICollection&lt;TValue&gt;t&gt;</see> will not
		///     affect this MultiDictionary.
		/// </summary>
		/// <returns>An IDictionary that copies the one inside of the MultiDictionary</returns>
		public IDictionary<TKey, ICollection<TValue>> ToDictionary()
		{
			return new Dictionary<TKey, ICollection<TValue>>(_dictionary, _dictionary.Comparer);
		}

		/// <summary>
		///     An enumerator for the key-value pairs of a MultiDictionary
		/// </summary>
		public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
		{
			private readonly MultiDictionary<TKey, TValue> _dictionary;

			private readonly int _version;

			private readonly IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> _keyEnumerator;

			private IEnumerator<TValue> _valuesEnumerator;

			private KeyValuePair<TKey, ICollection<TValue>> _currentListPair;

			private KeyValuePair<TKey, TValue> _currentValuePair;

			public KeyValuePair<TKey, TValue> Current
			{
				get { return _currentValuePair; }
			}

			Object IEnumerator.Current
			{
				get { return _currentValuePair; }
			}

			internal Enumerator(MultiDictionary<TKey, TValue> multiDictionary)
			{
				_dictionary = multiDictionary;
				_keyEnumerator = multiDictionary._dictionary.GetEnumerator();
				_currentListPair = _keyEnumerator.Current;
				_valuesEnumerator = null;
				_currentValuePair = new KeyValuePair<TKey, TValue>();
				_version = multiDictionary._version;
			}

			public void Dispose()
			{
				if (_keyEnumerator != null)
				{
					_keyEnumerator.Dispose();
				}
				if (_valuesEnumerator != null)
				{
					_valuesEnumerator.Dispose();
				}
			}

			~Enumerator()
			{
				Dispose();
			}

			public bool MoveNext()
			{
				if (_version != _dictionary._version)
				{
					throw new InvalidOperationException("Collection modified while enumerating");
				}
				if (_valuesEnumerator != null && _valuesEnumerator.MoveNext())
				{
					_currentValuePair = new KeyValuePair<TKey, TValue>(_currentListPair.Key, _valuesEnumerator.Current);
					return true;
				}
				if (MoveNextKey())
				{
					return MoveNext();
				}
				_currentValuePair = new KeyValuePair<TKey, TValue>();
				return false;
			}

			private bool MoveNextKey()
			{
				if (!_keyEnumerator.MoveNext())
				{
					return false;
				}
				_currentListPair = _keyEnumerator.Current;
				_valuesEnumerator = _currentListPair.Value.GetEnumerator();
				return true;
			}

			public void Reset()
			{
				if (_version != _dictionary._version)
				{
					throw new InvalidOperationException("Collection modified while enumerating");
				}
				_keyEnumerator.Reset();
				_currentListPair = _keyEnumerator.Current;
				_valuesEnumerator = null;
				_currentValuePair = new KeyValuePair<TKey, TValue>();
			}
		}

		/// <summary>
		///     An inner class that functions as a view of a ICollection within a MultiDictionary
		/// </summary>
		private class InnerCollectionView : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private readonly TKey _key;

			private readonly MultiDictionary<TKey, TValue> _multidictionary;

			public int Count
			{
				get
				{
					ICollection<TValue> tValues;
					if (!_multidictionary._dictionary.TryGetValue(_key, out tValues))
					{
						return 0;
					}
					return tValues.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					ICollection<TValue> tValues;
					if (!_multidictionary._dictionary.TryGetValue(_key, out tValues))
					{
						return _multidictionary.NewCollection(null).IsReadOnly;
					}
					return _multidictionary._dictionary[_key].IsReadOnly;
				}
			}

			public InnerCollectionView(MultiDictionary<TKey, TValue> multidictionary, TKey key)
			{
				_multidictionary = multidictionary;
				_key = key;
			}

			public void Add(TValue item)
			{
				_multidictionary.Add(_key, item);
			}

			public void Clear()
			{
				_multidictionary.Remove(_key);
			}

			public bool Contains(TValue item)
			{
				ICollection<TValue> tValues;
				if (!_multidictionary._dictionary.TryGetValue(_key, out tValues))
				{
					return false;
				}
				return tValues.Contains(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				ICollection<TValue> tValues;
				//Requires.NotNullAllowStructs<TValue[]>(array, "array");
				//Requires.Range(arrayIndex >= 0, "arrayIndex", null);
				//Requires.Range(arrayIndex <= (int) array.Length, "arrayIndex", null);
				if (_multidictionary._dictionary.TryGetValue(_key, out tValues))
				{
//					Requires.Argument((int) array.Length - arrayIndex >= tValues.Count, "arrayIndex", SR.GetString(Exceptions.CopyTo_ArgumentsTooSmall));
					tValues.CopyTo(array, arrayIndex);
				}
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return new Enumerator(_multidictionary, _key);
			}

			public bool Remove(TValue item)
			{
				return _multidictionary.RemoveItem(_key, item);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private class Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
			{
				private readonly MultiDictionary<TKey, TValue> _multiDictionary;

				private readonly int _version;

				private readonly IEnumerator<TValue> _enumerator;

				public TValue Current
				{
					get
					{
						if (_enumerator == null)
						{
							return default(TValue);
						}
						return _enumerator.Current;
					}
				}

				Object IEnumerator.Current
				{
					get { return Current; }
				}

				public Enumerator(MultiDictionary<TKey, TValue> multiDictionary, TKey key)
				{
					ICollection<TValue> tValues;
					_multiDictionary = multiDictionary;
					_version = multiDictionary._version;
					if (multiDictionary._dictionary.TryGetValue(key, out tValues))
					{
						_enumerator = tValues.GetEnumerator();
					}
				}

				public void Dispose()
				{
					if (_enumerator != null)
					{
						_enumerator.Dispose();
					}
				}

				~Enumerator()
				{
					Dispose();
				}

				public bool MoveNext()
				{
					if (_version != _multiDictionary._version)
					{
						throw new InvalidOperationException("Collection modified while enumerating");
					}
					if (_enumerator == null)
					{
						return false;
					}
					return _enumerator.MoveNext();
				}

				public void Reset()
				{
					if (_version != _multiDictionary._version)
					{
						throw new InvalidOperationException("Collection modified while enumerating");
					}
					if (_enumerator != null)
					{
						_enumerator.Reset();
					}
				}
			}
		}

		/// <summary>
		///     The collection type returned by MultiDictionary.Values. Functions mostly as an enumerator wrapper for the values
		/// </summary>
		private class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
		{
			private readonly MultiDictionary<TKey, TValue> _multiDictionary;

			public int Count
			{
				get { return _multiDictionary.Count; }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}

			public ValueCollection(MultiDictionary<TKey, TValue> multiDictionary)
			{
//				Requires.NotNullAllowStructs<MultiDictionary<TKey, TValue>>(multiDictionary, "multiDictionary");
				_multiDictionary = multiDictionary;
			}

			public void Add(TValue item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(TValue item)
			{
				return _multiDictionary.ContainsValue(item);
			}

			public void CopyTo(TValue[] array, int arrayIndex)
			{
				//Requires.NotNullAllowStructs<TValue[]>(array, "array");
				//Requires.Range(arrayIndex >= 0, "arrayIndex", null);
				//Requires.Range(arrayIndex <= (int) array.Length, "arrayIndex", null);
				//Requires.Argument((int) array.Length - arrayIndex >= this.Count, "arrayIndex", SR.GetString(Exceptions.CopyTo_ArgumentsTooSmall));
				foreach (var tValue in this)
				{
					var num = arrayIndex;
					arrayIndex = num + 1;
					array[num] = tValue;
				}
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return new ValueCollectionEnumerator(_multiDictionary);
			}

			public bool Remove(TValue item)
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private class ValueCollectionEnumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

				private bool valid;

				public TValue Current
				{
					get
					{
						if (!valid)
						{
							return default(TValue);
						}
						return _enumerator.Current.Value;
					}
				}

				Object IEnumerator.Current
				{
					get { return Current; }
				}

				internal ValueCollectionEnumerator(MultiDictionary<TKey, TValue> multidictionary)
				{
					_enumerator = multidictionary.GetEnumerator();
					valid = false;
				}

				public void Dispose()
				{
					if (_enumerator != null)
					{
						_enumerator.Dispose();
					}
				}

				~ValueCollectionEnumerator()
				{
					Dispose();
				}

				public bool MoveNext()
				{
					valid = _enumerator.MoveNext();
					return valid;
				}

				public void Reset()
				{
					_enumerator.Reset();
					valid = false;
				}
			}
		}
	}
}