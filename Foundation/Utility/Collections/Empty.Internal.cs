// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Std.Utility.Collections
{
	public partial class Empty
    {
        private static class Internal
        {
            internal static class InternalArray<T>
            {
                public static readonly T[] Instance = new T[0];
            }

			internal class InternalCollection<T> : InternalEnumerable<T>, ICollection<T>
			{
				public static readonly ICollection<T> Instance = new InternalCollection<T>();

				protected InternalCollection()
				{
				}

				public void Add(T item)
				{
					throw new NotSupportedException();
				}

				public void Clear()
				{
				}

				public bool Contains(T item)
				{
					return false;
				}

				public void CopyTo(T[] array, int arrayIndex)
				{
				}

				public int Count
				{
					get
					{
						return 0;
					}
				}

				public bool IsReadOnly
				{
					get
					{
						return true;
					}
				}

				public bool Remove(T item)
				{
					throw new NotSupportedException();
				}
			}

			internal class InternalDictionary<TKey, TValue> : InternalCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
			{
				public static readonly new IDictionary<TKey, TValue> Instance = new InternalDictionary<TKey, TValue>();

				private InternalDictionary()
				{
				}

				public void Add(TKey key, TValue value)
				{
					throw new NotSupportedException();
				}

				public bool ContainsKey(TKey key)
				{
					return false;
				}

				public ICollection<TKey> Keys
				{
					get
					{
						return InternalCollection<TKey>.Instance;
					}
				}

				public bool Remove(TKey key)
				{
					throw new NotSupportedException();
				}

				public bool TryGetValue(TKey key, out TValue value)
				{
					value = default(TValue);
					return false;
				}

				public ICollection<TValue> Values
				{
					get
					{
						return InternalCollection<TValue>.Instance;
					}
				}

				public TValue this[TKey key]
				{
					get
					{
						throw new NotSupportedException();
					}

					set
					{
						throw new NotSupportedException();
					}
				}
			}

			internal class InternalEnumerable<T> : IEnumerable<T>
			{
				// PERF: cache the instance of enumerator. 
				// accessing a generic static field is kinda slow from here,
				// but since empty enumerables are singletons, there is no harm in having 
				// one extra instance field
				private readonly IEnumerator<T> _enumerator = InternalEnumerator<T>.Instance;

				public IEnumerator<T> GetEnumerator()
				{
					return _enumerator;
				}

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}
			}

			internal class InternalEnumerator : IEnumerator
			{
				public static readonly IEnumerator Instance = new InternalEnumerator();

				protected InternalEnumerator()
				{
				}

				public object Current
				{
					get
					{
						throw new InvalidOperationException();
					}
				}

				public bool MoveNext()
				{
					return false;
				}

				public void Reset()
				{
					throw new InvalidOperationException();
				}
			}

			internal class InternalEnumerator<T> : InternalEnumerator, IEnumerator<T>
			{
				public static new readonly IEnumerator<T> Instance = new InternalEnumerator<T>();

				protected InternalEnumerator()
				{
				}

				public new T Current
				{
					get
					{
						throw new InvalidOperationException();
					}
				}

				public void Dispose()
				{
				}
			}

			internal class InternalList<T> : InternalCollection<T>, IList<T>, IReadOnlyList<T>
			{
				public static readonly new InternalList<T> Instance = new InternalList<T>();

				protected InternalList()
				{
				}

				public int IndexOf(T item)
				{
					return -1;
				}

				public void Insert(int index, T item)
				{
					throw new NotSupportedException();
				}

				public void RemoveAt(int index)
				{
					throw new NotSupportedException();
				}

				public T this[int index]
				{
					get
					{
						throw new ArgumentOutOfRangeException("index");
					}

					set
					{
						throw new NotSupportedException();
					}
				}
			}

			internal class InternalSet<T> : InternalCollection<T>, ISet<T>
			{
				public static readonly new ISet<T> Instance = new InternalSet<T>();

				protected InternalSet()
				{
				}

				public new bool Add(T item)
				{
					throw new NotImplementedException();
				}

				public void ExceptWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void IntersectWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsProperSubsetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsProperSupersetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsSubsetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool IsSupersetOf(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool Overlaps(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public bool SetEquals(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void SymmetricExceptWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public void UnionWith(IEnumerable<T> other)
				{
					throw new NotImplementedException();
				}

				public new System.Collections.IEnumerator GetEnumerator()
				{
					return Instance.GetEnumerator();
				}
			}
		}
    }
}
