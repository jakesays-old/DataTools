using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Std.Utility.Collections
{
	[DebuggerTypeProxy(typeof(HybridDictionaryDebugView<,>))]
	[DebuggerDisplay("Count = {Count}, Mode = {_mode}")]
	[Serializable]
	public class HybridDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, ISerializable, IDeserializationCallback
	{
		private enum Mode
		{
			Unknown,
			Hash,
			List
		}

		private const int HashThreshold = 10;

		private struct Entry
		{
			public int HashCode;    // Lower 31 bits of hash code, -1 if unused 
			public int Next;        // Index of next entry, -1 if last
			public TKey Key;           // Key of entry 
			public TValue Value;         // Value of entry
		}

		private int[] _buckets;
		private Entry[] _entries;
		private int _count;
		private int _version;
		private int _freeList;
		private int _freeCount;
		private IEqualityComparer<TKey> _comparer;
		private KeyCollection _keys;
		private ValueCollection _values;
		private object _syncRoot;
		private Mode _mode;

		private SerializationInfo _serializationInfo; //A temporary variable which we need during deserialization. 

		// constants for serialization
		private const string VersionName = "Version";
		private const string HashSizeName = "HashSize";  // Must save buckets.Length
		private const string KeyValuePairsName = "KeyValuePairs";
		private const string ComparerName = "Comparer";
		private const string ModeName = "Mode";
		private const string CountName = "Count";

		public HybridDictionary() : this(0, null)
		{ }

		public HybridDictionary(int capacity) : this(capacity, null) 
		{ }

		public HybridDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer) 
		{ }

		public HybridDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity > 0)
			{
				Initialize(capacity);
			}
			
			_comparer = comparer ?? EqualityComparer<TKey>.Default;
		}

		public HybridDictionary(IDictionary<TKey, TValue> dictionary) 
			: this(dictionary, null) 
		{ }

		public HybridDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) 
			: this(dictionary != null ? dictionary.Count : 0, comparer)
		{

			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}

			foreach (KeyValuePair<TKey, TValue> pair in dictionary)
			{
				Add(pair.Key, pair.Value);
			}
		}

		protected HybridDictionary(SerializationInfo info, StreamingContext context)
		{
			//We can't do anything with the keys and values until the entire graph has been deserialized
			//and we have a resonable estimate that GetHashCode is not going to fail.  For the time being, 
			//we'll just cache this.  The graph is not valid until OnDeserialization has been called. 
			_serializationInfo = info;
		}

		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return _comparer;
			}
		}

		public int Count
		{
			get { return _count - _freeCount; }
		}

		public KeyCollection Keys
		{
			get
			{
				Contract.Ensures(Contract.Result<KeyCollection>() != null);
				if (_keys == null)
				{
					_keys = new KeyCollection(this);
				}
				return _keys;
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get
			{
				if (_keys == null)
				{
					_keys = new KeyCollection(this);
				}
				return _keys;
			}
		}

		public ValueCollection Values
		{
			get
			{
				Contract.Ensures(Contract.Result<ValueCollection>() != null);
				if (_values == null)
				{
					_values = new ValueCollection(this);
				}
				return _values;
			}
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
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

		public TValue this[TKey key]
		{
			get
			{
				int i = FindEntry(key);
				if (i >= 0)
				{
					return _entries[i].Value;
				}
				throw new KeyNotFoundException();
			}
			set
			{
				Insert(key, value, false);
			}
		}

		public void Add(TKey key, TValue value)
		{
			Insert(key, value, true);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			Add(keyValuePair.Key, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			int i = FindEntry(keyValuePair.Key);
			if (i >= 0 && EqualityComparer<TValue>.Default.Equals(_entries[i].Value, keyValuePair.Value))
			{
				return true;
			}
			return false;
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			var i = FindEntry(keyValuePair.Key);
			if (i >= 0 && 
				(_mode == Mode.List || EqualityComparer<TValue>.Default.Equals(_entries[i].Value, keyValuePair.Value)))
			{
				Remove(keyValuePair.Key);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			if (_count > 0)
			{
				_mode = Mode.List;
				for (int i = 0; i < _buckets.Length; i++)
				{
					_buckets[i] = -1;
				}

				Array.Clear(_entries, 0, _count);
				_freeList = -1;
				_count = 0;
				_freeCount = 0;
				_version++;
			}
		}

		public bool ContainsKey(TKey key)
		{
			return FindEntry(key) >= 0;
		}

		public bool ContainsValue(TValue value)
		{
			if (value == null)
			{
				for (int i = 0; i < _count; i++)
				{
					if (_mode == Mode.List && _entries[i].Value == null)
					{
						return true;
					}

					if (_entries[i].HashCode >= 0 && _entries[i].Value == null)
					{
						return true;
					}
				}
			}
			else
			{
				EqualityComparer<TValue> c = EqualityComparer<TValue>.Default;
				for (int i = 0; i < _count; i++)
				{
					if (_mode == Mode.List && c.Equals(_entries[i].Value, value))
					{
						return true;
					}

					if (_entries[i].HashCode >= 0 && c.Equals(_entries[i].Value, value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (index < 0 || index > array.Length)
			{
				throw new ArgumentOutOfRangeException("array", "Non-negative number required.");
			}

			if (array.Length - index < Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}

			var count = _count;
			var entries = _entries;
			for (var i = 0; i < count; i++)
			{
				if (_mode == Mode.List || entries[i].HashCode >= 0)
				{
					array[index++] = new KeyValuePair<TKey, TValue>(entries[i].Key, entries[i].Value);
				}
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this, Enumerator.KeyValuePair);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator(this, Enumerator.KeyValuePair);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			info.AddValue(ModeName, (int) _mode);
			info.AddValue(VersionName, _version);
			info.AddValue(ComparerName, _comparer, typeof(IEqualityComparer<TKey>));
			
			var addEntries = false;

			if (_mode == Mode.List)
			{
				info.AddValue(CountName, _count);
				addEntries = _count > 0;
			}
			else
			{
				info.AddValue(HashSizeName, _buckets == null ? 0 : _buckets.Length); //This is the length of the bucket array.

				addEntries = _buckets != null;
			}

			if (addEntries)
			{
				var array = new KeyValuePair<TKey, TValue>[Count];
				CopyTo(array, 0);
				info.AddValue(KeyValuePairsName, array, typeof(KeyValuePair<TKey, TValue>[]));
			}
		}

		private int FindEntry(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			if (_mode == Mode.List)
			{
				if (_entries != null)
				{
					for (var idx = 0; idx < _count; idx++)
					{
						if (_comparer.Equals(_entries[idx].Key, key))
						{
							return idx;
						}
					}
				}

				return -1;
			}

			if (_buckets != null)
			{
				int hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
				for (int i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].Next)
				{
					if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
					{
						return i;
					}
				}
			}
			return -1;
		}

		private void Initialize(int capacity, bool forceHashMode = false)
		{			
			if (!forceHashMode && capacity <= HashThreshold)
			{
				_entries = new Entry[HashThreshold];
				_mode = Mode.List;
				return;
			}

			_mode = Mode.Hash;

			var size = HashHelpers.GetPrime(capacity);
			_buckets = new int[size];
			for (var i = 0; i < _buckets.Length; i++)
			{
				_buckets[i] = -1;
			}
			_entries = new Entry[size];
			_freeList = -1;
		}

		private void SwitchModes()
		{
			//TODO: figure out what the correct size should be
			//so we dont end up expanding immediately after initializing

			var oldEntries = _entries;
			Initialize(HashThreshold + 1, true);

			foreach(var entry in oldEntries)
			{
				Insert(entry.Key, entry.Value, false);
			}
		}

		private void Insert(TKey key, TValue value, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			if (_mode != Mode.List && _buckets == null)
			{
				Initialize(0);
			}

			if (_mode == Mode.List)
			{
				var existingEntryIdx = -1;
				for (var idx = 0; idx < _count; idx++)
				{
					if (_comparer.Equals(_entries[idx].Key, key))
					{
						existingEntryIdx = idx;
						break;
					}
				}

				if (existingEntryIdx != -1)
				{
					if (add)
					{
						throw new ArgumentException("An item with the same key has already been added.");
					}

					_entries[existingEntryIdx].Value = value;
					_version += 1;
					return;
				}

				if (_count == HashThreshold)
				{
					SwitchModes();
					Insert(key, value, true);
					return;
				}

				_entries[_count].Key = key;
				_entries[_count].Value = value;
				_count += 1;
				_version += 1;

				return;
			}

			var hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
			var targetBucket = hashCode % _buckets.Length;
			for (var i = _buckets[targetBucket]; i >= 0; i = _entries[i].Next)
			{
				if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
				{
					if (add)
					{
						throw new ArgumentException("An item with the same key has already been added.");
					}
					_entries[i].Value = value;
					_version++;
					return;
				}
			}
			int index;
			if (_freeCount > 0)
			{
				index = _freeList;
				_freeList = _entries[index].Next;
				_freeCount--;
			}
			else
			{
				if (_count == _entries.Length)
				{
					Resize();
					targetBucket = hashCode % _buckets.Length;
				}
				index = _count;
				_count++;
			}

			_entries[index].HashCode = hashCode;
			_entries[index].Next = _buckets[targetBucket];
			_entries[index].Key = key;
			_entries[index].Value = value;
			_buckets[targetBucket] = index;
			_version++;
		}

		public virtual void OnDeserialization(object sender)
		{
			if (_serializationInfo == null)
			{
				// It might be necessary to call OnDeserialization from a container if the container object also implements
				// OnDeserialization. However, remoting will call OnDeserialization again. 
				// We can return immediately if this function is called twice.
				// Note we set m_siInfo to null at the end of this method.
				return;
			}

			_mode = (Mode) _serializationInfo.GetInt32(ModeName);
			var realVersion = _serializationInfo.GetInt32(VersionName);
			var hashsize = _serializationInfo.GetInt32(HashSizeName);
			_comparer = (IEqualityComparer<TKey>) _serializationInfo.GetValue(ComparerName, typeof(IEqualityComparer<TKey>));

			var deserializeEntries = false;

			if (_mode == Mode.List)
			{
				var count = _serializationInfo.GetInt32(CountName);
				Initialize(count);

				deserializeEntries = count > 0;
			}
			else
			{
				if (hashsize != 0)
				{
					_buckets = new int[hashsize];
					for (int i = 0; i < _buckets.Length; i++)
					{
						_buckets[i] = -1;
					}
					_entries = new Entry[hashsize];
					_freeList = -1;

					deserializeEntries = true;
				}
				else
				{
					_buckets = null;
				}
			}

			if (deserializeEntries)
			{
				var array = (KeyValuePair<TKey, TValue>[])
					_serializationInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

				if (array == null)
				{
					throw new SerializationException("The keys for this dictionary are missing");
				}

				for (var i = 0; i < array.Length; i++)
				{
					if (array[i].Key == null)
					{
						throw new SerializationException("One of the serialized keys is null");
					}
					Insert(array[i].Key, array[i].Value, true);
				}
			}

			_version = realVersion;
			_serializationInfo = null;
		}

		//only called in hash mode
		private void Resize()
		{
			var newSize = HashHelpers.GetPrime(_count * 2);
			var newBuckets = new int[newSize];
			for (var i = 0; i < newBuckets.Length; i++)
			{
				newBuckets[i] = -1;
			}

			var newEntries = new Entry[newSize];
			Array.Copy(_entries, 0, newEntries, 0, _count);
			for (var i = 0; i < _count; i++)
			{
				var bucket = newEntries[i].HashCode % newSize;
				newEntries[i].Next = newBuckets[bucket];
				newBuckets[bucket] = i;
			}

			_buckets = newBuckets;
			_entries = newEntries;
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			//currently remove doesnt switch back to list mode if the size shrinks below HashThreshold
			if (_mode == Mode.List)
			{
				//TODO: optimize this
				var remainingEntries = new Entry[HashThreshold];
				var remainingCount = 0;
				for (var idx = 0; idx < _count; idx++)
				{
					if (!_comparer.Equals(_entries[idx].Key, key))
					{
						remainingEntries[remainingCount++] = _entries[idx];
					}
				}

				var isSmaller = _count != remainingCount;
				if (isSmaller)
				{
					_entries = remainingEntries;
					_count = remainingCount;
					_version += 1;					
				}

				return isSmaller;
			}

			if (_buckets != null)
			{
				var hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
				var bucket = hashCode % _buckets.Length;
				var last = -1;

				for (var i = _buckets[bucket]; i >= 0; last = i, i = _entries[i].Next)
				{
					if (_entries[i].HashCode == hashCode && _comparer.Equals(_entries[i].Key, key))
					{
						if (last < 0)
						{
							_buckets[bucket] = _entries[i].Next;
						}
						else
						{
							_entries[last].Next = _entries[i].Next;
						}
						_entries[i].HashCode = -1;
						_entries[i].Next = _freeList;
						_entries[i].Key = default(TKey);
						_entries[i].Value = default(TValue);
						_freeList = i;
						_freeCount++;
						_version++;
						return true;
					}
				}
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var i = FindEntry(key);
			if (i >= 0)
			{
				value = _entries[i].Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		// This is a convenience method for the internal callers that were converted from using Hashtable. 
		// Many were combining key doesn't exist and key exists but null value (for non-value types) checks.
		// This allows them to continue getting that behavior with minimal code delta. This is basically 
		// TryGetValue without the out param 
		public TValue GetValueOrDefault(TKey key)
		{
			var i = FindEntry(key);
			if (i >= 0)
			{
				return _entries[i].Value;
			}
			return default(TValue);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return false; }
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
			}

			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("The lower bound of target array must be zero.");
			}

			if (index < 0 || index > array.Length)
			{
				throw new ArgumentOutOfRangeException("'index' must be non-negative.");
			}

			if (array.Length - index < Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}

			var pairs = array as KeyValuePair<TKey, TValue>[];
			if (pairs != null)
			{
				CopyTo(pairs, index);
			}
			else if (array is DictionaryEntry[])
			{
				var entries = _entries;
				var dictEntryArray = array as DictionaryEntry[];

				for (var i = 0; i < _count; i++)
				{
					if (_mode == Mode.List || entries[i].HashCode >= 0)
					{
						dictEntryArray[index++] = new DictionaryEntry(entries[i].Key, entries[i].Value);
					}
				}
			}
			else
			{
				var objects = array as object[];
				if (objects == null)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
				}

				try
				{
					var count = _count;
					var entries = _entries;
					for (var i = 0; i < count; i++)
					{
						if (_mode == Mode.List || entries[i].HashCode >= 0)
						{
							objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].Key, entries[i].Value);
						}
					}
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this, Enumerator.KeyValuePair);
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (_syncRoot == null)
				{
					System.Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
				}
				return _syncRoot;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get { return false; }
		}

		bool IDictionary.IsReadOnly
		{
			get { return false; }
		}

		ICollection IDictionary.Keys
		{
			get { return (ICollection) Keys; }
		}

		ICollection IDictionary.Values
		{
			get { return (ICollection) Values; }
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (IsCompatibleKey(key))
				{
					int i = FindEntry((TKey) key);
					if (i >= 0)
					{
						return _entries[i].Value;
					}
				}
				return null;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				
				IfNullAndNullsAreIllegalThenThrow<TValue>(value, "value");

				try
				{
					TKey tempKey = (TKey) key;
					try
					{
						this[tempKey] = (TValue) value;
					}
					catch (InvalidCastException)
					{
						ThrowWrongValueTypeArgumentException(value, typeof(TValue));
					}
				}
				catch (InvalidCastException)
				{
					ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
				}
			}
		}

		[TerminatesProgram]
		private static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
		{
			throw new ArgumentException(
				"The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.".FormatWith(key, targetType),
				"key");
		}

		[TerminatesProgram]
		private static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
		{
			throw new ArgumentException(
				"The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.".FormatWith(value, targetType),
				"value");
		}

		private static void IfNullAndNullsAreIllegalThenThrow<T>(object value, string argName)
		{
			// Note that default(T) is not equal to null for value types except when T is Nullable<U>.
			if (value == null && !(default(T) == null))
			{
				throw new ArgumentNullException(argName);
			}
		}

		[TerminatesProgram]
		private static void ThrowEnumFailedVersion()
		{
			throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
		}

		[TerminatesProgram]
		private static void ThrowEnumOpCantHappen()
		{
			throw new InvalidOperationException("Enumeration has either not started or has already finished.");
		}

		private static bool IsCompatibleKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return (key is TKey);
		}

		void IDictionary.Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			IfNullAndNullsAreIllegalThenThrow<TValue>(value, "value");

			try
			{
				TKey tempKey = (TKey) key;

				try
				{
					Add(tempKey, (TValue) value);
				}
				catch (InvalidCastException)
				{
					ThrowWrongValueTypeArgumentException(value, typeof(TValue));
				}
			}
			catch (InvalidCastException)
			{
				ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
			}
		}

		bool IDictionary.Contains(object key)
		{
			if (IsCompatibleKey(key))
			{
				return ContainsKey((TKey) key);
			}

			return false;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Enumerator(this, Enumerator.DictEntry);
		}

		void IDictionary.Remove(object key)
		{
			if (IsCompatibleKey(key))
			{
				Remove((TKey) key);
			}
		}

		[Serializable]
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>,
			IDictionaryEnumerator
		{
			private readonly HybridDictionary<TKey, TValue> _dictionary;
			private readonly int _version;
			private int _index;
			private KeyValuePair<TKey, TValue> _current;
			private readonly int _getEnumeratorRetType;  // What should Enumerator.Current return?

			internal const int DictEntry = 1;
			internal const int KeyValuePair = 2;

			internal Enumerator(HybridDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
			{
				_dictionary = dictionary;
				_version = dictionary._version;
				_index = 0;
				_getEnumeratorRetType = getEnumeratorRetType;
				_current = new KeyValuePair<TKey, TValue>();
			}

			public bool MoveNext()
			{
				if (_version != _dictionary._version)
				{
					ThrowEnumFailedVersion();
				}

				// Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
				// dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
				while ((uint) _index < (uint) _dictionary._count)
				{
					if (_dictionary._mode == Mode.List || _dictionary._entries[_index].HashCode >= 0)
					{
						_current = new KeyValuePair<TKey, TValue>(_dictionary._entries[_index].Key, _dictionary._entries[_index].Value);
						_index++;
						return true;
					}
					_index++;
				}

				_index = _dictionary._count + 1;
				_current = new KeyValuePair<TKey, TValue>();
				return false;
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get { return _current; }
			}

			public void Dispose()
			{
			}

			object IEnumerator.Current
			{
				get
				{
					if (_index == 0 || (_index == _dictionary._count + 1))
					{
						ThrowEnumOpCantHappen();
					}

					if (_getEnumeratorRetType == DictEntry)
					{
						return new DictionaryEntry(_current.Key, _current.Value);
					}
					else
					{
						return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
					}
				}
			}

			void IEnumerator.Reset()
			{
				if (_version != _dictionary._version)
				{
					ThrowEnumFailedVersion();
				}

				_index = 0;
				_current = new KeyValuePair<TKey, TValue>();
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					if (_index == 0 || (_index == _dictionary._count + 1))
					{
						ThrowEnumOpCantHappen();
					}

					return new DictionaryEntry(_current.Key, _current.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					if (_index == 0 || (_index == _dictionary._count + 1))
					{
						ThrowEnumOpCantHappen();
					}

					return _current.Key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					if (_index == 0 || (_index == _dictionary._count + 1))
					{
						ThrowEnumOpCantHappen();
					}

					return _current.Value;
				}
			}
		}

		[DebuggerTypeProxy(typeof(HybridDictionaryKeyCollectionDebugView<,>))]
		[DebuggerDisplay("Count = {Count}")]
		[Serializable]
		public sealed class KeyCollection : ICollection<TKey>, ICollection
		{
			private readonly HybridDictionary<TKey, TValue> _dictionary;

			public KeyCollection(HybridDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				_dictionary = dictionary;
			}

			public KeyEnumerator GetEnumerator()
			{
				return new KeyEnumerator(_dictionary);
			}

			public void CopyTo(TKey[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if (index < 0 || index > array.Length)
				{
					throw new ArgumentOutOfRangeException("array", "Non-negative number required.");
				}

				if (array.Length - index < _dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}

				var count = _dictionary._count;
				var entries = _dictionary._entries;
				for (var i = 0; i < count; i++)
				{
					if (_dictionary._mode == Mode.List || entries[i].HashCode >= 0)
					{
						array[index++] = entries[i].Key;
					}
				}
			}

			public int Count
			{
				get { return _dictionary.Count; }
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get { return true; }
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				return _dictionary.ContainsKey(item);
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
			{
				return new KeyEnumerator(_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new KeyEnumerator(_dictionary);
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
				}

				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.");
				}

				if (index < 0 || index > array.Length)
				{
					throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
				}

				if (array.Length - index < _dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}

				TKey[] keys = array as TKey[];
				if (keys != null)
				{
					CopyTo(keys, index);
				}
				else
				{
					object[] objects = array as object[];
					if (objects == null)
					{
						throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
					}

					var count = _dictionary._count;
					var entries = _dictionary._entries;
					try
					{
						for (var i = 0; i < count; i++)
						{
							if (_dictionary._mode == Mode.List || entries[i].HashCode >= 0)
							{
								objects[index++] = entries[i].Key;
							}
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
					}
				}
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			object ICollection.SyncRoot
			{
				get { return ((ICollection) _dictionary).SyncRoot; }
			}

			[Serializable]
			public struct KeyEnumerator : IEnumerator<TKey>, IEnumerator
			{
				private readonly HybridDictionary<TKey, TValue> _dictionary;
				private int _index;
				private readonly int _version;
				private TKey _currentKey;

				internal KeyEnumerator(HybridDictionary<TKey, TValue> dictionary)
				{
					_dictionary = dictionary;
					_version = dictionary._version;
					_index = 0;
					_currentKey = default(TKey);
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (_version != _dictionary._version)
					{
						ThrowEnumFailedVersion();
					}

					while ((uint) _index < (uint) _dictionary._count)
					{
						if (_dictionary._mode == Mode.List || _dictionary._entries[_index].HashCode >= 0)
						{
							_currentKey = _dictionary._entries[_index].Key;
							_index++;
							return true;
						}
						_index++;
					}

					_index = _dictionary._count + 1;
					_currentKey = default(TKey);
					return false;
				}

				public TKey Current
				{
					get
					{
						return _currentKey;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						if (_index == 0 || (_index == _dictionary._count + 1))
						{
							ThrowEnumOpCantHappen();
						}

						return _currentKey;
					}
				}

				void IEnumerator.Reset()
				{
					if (_version != _dictionary._version)
					{
						ThrowEnumFailedVersion();
					}

					_index = 0;
					_currentKey = default(TKey);
				}
			}
		}

		[DebuggerTypeProxy(typeof(HybridDictionaryValueCollectionDebugView<,>))]
		[DebuggerDisplay("Count = {Count}")]
		[Serializable]
		public sealed class ValueCollection : ICollection<TValue>, ICollection
		{
			private readonly HybridDictionary<TKey, TValue> _dictionary;

			public ValueCollection(HybridDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				_dictionary = dictionary;
			}

			public ValueEnumerator GetEnumerator()
			{
				return new ValueEnumerator(_dictionary);
			}

			public void CopyTo(TValue[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if (index < 0 || index > array.Length)
				{
					throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
				}

				if (array.Length - index < _dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}

				var count = _dictionary._count;
				var entries = _dictionary._entries;
				for (var i = 0; i < count; i++)
				{
					if (_dictionary._mode == Mode.List || entries[i].HashCode >= 0)
					{
						array[index++] = entries[i].Value;
					}
				}
			}

			public int Count
			{
				get { return _dictionary.Count; }
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get { return true; }
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				return _dictionary.ContainsValue(item);
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return new ValueEnumerator(_dictionary);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new ValueEnumerator(_dictionary);
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
				}

				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.");
				}

				if (index < 0 || index > array.Length)
				{
					throw new ArgumentOutOfRangeException("'index' must be non-negative.");
				}

				if (array.Length - index < _dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}

				var values = array as TValue[];
				if (values != null)
				{
					CopyTo(values, index);
				}
				else
				{
					var objects = array as object[];
					if (objects == null)
					{
						throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
					}

					var count = _dictionary._count;
					var entries = _dictionary._entries;
					try
					{
						for (var i = 0; i < count; i++)
						{
							if (_dictionary._mode == Mode.List || entries[i].HashCode >= 0)
							{
								objects[index++] = entries[i].Value;
							}
						}
					}
					catch (ArrayTypeMismatchException)
					{
						throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
					}
				}
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			object ICollection.SyncRoot
			{
				get { return ((ICollection) _dictionary).SyncRoot; }
			}

			[Serializable]
			public struct ValueEnumerator : IEnumerator<TValue>, IEnumerator
			{
				private readonly HybridDictionary<TKey, TValue> _dictionary;
				private int _index;
				private readonly int _version;
				private TValue _currentValue;

				internal ValueEnumerator(HybridDictionary<TKey, TValue> dictionary)
				{
					_dictionary = dictionary;
					_version = dictionary._version;
					_index = 0;
					_currentValue = default(TValue);
				}

				public void Dispose()
				{
				}

				public bool MoveNext()
				{
					if (_version != _dictionary._version)
					{
						ThrowEnumFailedVersion();
					}

					while ((uint) _index < (uint) _dictionary._count)
					{
						if (_dictionary._mode == Mode.List || _dictionary._entries[_index].HashCode >= 0)
						{
							_currentValue = _dictionary._entries[_index].Value;
							_index++;
							return true;
						}
						_index++;
					}
					_index = _dictionary._count + 1;
					_currentValue = default(TValue);
					return false;
				}

				public TValue Current
				{
					get
					{
						return _currentValue;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						if (_index == 0 || (_index == _dictionary._count + 1))
						{
							ThrowEnumOpCantHappen();
						}

						return _currentValue;
					}
				}

				void IEnumerator.Reset()
				{
					if (_version != _dictionary._version)
					{
						ThrowEnumFailedVersion();
					}
					_index = 0;
					_currentValue = default(TValue);
				}
			}
		}
	}

	internal static class HashHelpers
	{
		// Table of prime numbers to use as hash table sizes. 
		// A typical resize algorithm would pick the smallest prime number in this array
		// that is larger than twice the previous capacity. 
		// Suppose our Hashtable currently has capacity x and enough elements are added 
		// such that a resize needs to occur. Resizing first computes 2x then finds the
		// first prime in the table greater than 2x, i.e. if primes are ordered 
		// p_1, p_2, , p_i,, it finds p_n such that p_n-1 < 2x < p_n.
		// Doubling is important for preserving the asymptotic complexity of the
		// hashtable operations such as add.  Having a prime guarantees that double
		// hashing does not lead to infinite loops.  IE, your hash function will be 
		// h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
		internal static readonly int[] Primes = { 
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

		internal static bool IsPrime(int candidate)
		{
			if ((candidate & 1) != 0)
			{
				var limit = (int) Math.Sqrt(candidate);
				for (var divisor = 3; divisor <= limit; divisor += 2)
				{
					if ((candidate % divisor) == 0)
					{
						return false;
					}
				}
				return true;
			}
			return (candidate == 2);
		}

		internal static int GetPrime(int min)
		{
			if (min < 0)
			{
				throw new ArgumentException("Dictionary's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.");
			}
			Contract.EndContractBlock();

			for (var i = 0; i < Primes.Length; i++)
			{
				var prime = Primes[i];
				if (prime >= min)
				{
					return prime;
				}
			}

			//outside of our predefined table. 
			//compute the hard way.
			for (var i = (min | 1); i < Int32.MaxValue; i += 2)
			{
				if (IsPrime(i))
				{
					return i;
				}
			}
			return min;
		}
	} 

}
