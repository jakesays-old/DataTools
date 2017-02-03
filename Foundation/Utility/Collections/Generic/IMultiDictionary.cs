using System.Collections;
using System.Collections.Generic;

namespace Std.Utility.Collections.Generic
{
	/// <summary>
	///     An IMultiDictionary is an interface describing the methods to be
	///     implemented by the MultiDictionary collection class.
	///     A MultiDictionary can be viewed as a <see cref="T:System.Collections.IDictionary" /> that allows multiple
	///     values for any given unique key. While the MultiDictionary API is
	///     mostly the same as that of a regular <see cref="T:System.Collections.IDictionary" />, there is a distinction
	///     in that getting the value for a key returns a <see cref="T:System.Collections.Generic.ICollection`1" /> of values
	///     rather than a single value associated with that key. Additionally,
	///     there is functionality to allow adding or removing more than a single
	///     value at once.
	///     The MultiDictionary can also be viewed as a
	///     <see cref="T:System.Collections.IDictionary">IDictionary&lt;TKey,ICollection&lt;TValue&gt;t&gt;</see>
	///     where the <see cref="T:System.Collections.Generic.ICollection`1" /> is abstracted from the view of the programmer
	///     using
	///     the MultiDictionary. However, the MultiDictionary is distinct from the
	///     <see cref="T:System.Collections.IDictionary">IDictionary&lt;TKey,ICollection&lt;TValue&gt;t&gt;</see> in that the
	///     MultiDictionary treats every
	///     item within the <see cref="T:System.Collections.Generic.ICollection`1" /> as a member of its own key-value pair.
	///     This
	///     distinction allows for iteration over the key-value pairs, rather than
	///     over KeyValuePair&lt;TKey,ICollection&lt;TValue&gt;t&gt;, and also affects the size(), contains(),
	///     and setter methods, as well as the Values Property. More specific descriptions
	///     are included with each method/Property.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public interface IMultiDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>,
		IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
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
		ICollection<TValue> this[TKey key] { get; }

		/// <summary>
		///     Gets a collection of all of the individual keys. Will only return keys that
		///     have one or more associated values.
		/// </summary>
		ICollection<TKey> Keys { get; }

		/// <summary>
		///     Gets a collection of all of the individual values in this MultiDictionary
		/// </summary>
		ICollection<TValue> Values { get; }

		/// <summary>
		///     Adds the specified key and value to the MultiDictionary.
		/// </summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add.</param>
		/// <remarks>
		///     Unlike the Add for Dictionary, the MultiDictionary Add will not
		///     throw any exceptions. If the given key is already in the MultiDictionary,
		///     then value will be added to that keys associated values collection.
		/// </remarks>
		void Add(TKey key, TValue value);

		/// <summary>
		///     Adds a number of key-value pairs to this MultiDictionary, where
		///     the key for each value is the key param, and the value for a pair
		///     is an element from "values"
		/// </summary>
		/// <param name="key">The key of all entries to add</param>
		/// <param name="values">An IEnumerable of values to add</param>
		void AddRange(TKey key, IEnumerable<TValue> values);

		/// <summary>
		///     Determines if the given key-value pair exists within the MultiDictionary
		/// </summary>
		/// <param name="key">The key to check for</param>
		/// <param name="value">The value to check for</param>
		/// <returns>True if the MultiDictionary contains the requested pair, false otherwise</returns>
		bool Contains(TKey key, TValue value);

		/// <summary>
		///     Determines if the given key exists within this MultiDictionary and has
		///     at least one value associated with it.
		/// </summary>
		/// <param name="key">The key to search the dictionary for</param>
		/// <returns>True if the MultiDictionary contains the requested key, false otherwise</returns>
		bool ContainsKey(TKey key);

		/// <summary>
		///     Determines if the given value exists within the MultiDictionary
		/// </summary>
		/// <param name="value">A value to search the MultiDictionary for</param>
		/// <returns>True if the MultiDictionary contains the requested value, false otherwise</returns>
		bool ContainsValue(TValue value);

		/// <summary>
		///     Removes all values associated with the given key from the MultiDictionary
		/// </summary>
		/// <param name="key">The key of the items to be removed</param>
		/// <returns>True if the removal was successful, false otherwise</returns>
		bool Remove(TKey key);

		/// <summary>
		///     Removes the first instance (if any) of the given key-value pair from the MultiDictionary.
		///     If the item being removed is the last one associated with its key, that key will be removed
		///     from the dictionary and its associated values collection will be freed as if a call to Remove(key)
		///     had been made.
		/// </summary>
		/// <param name="key">The key of the item to remove</param>
		/// <param name="value">The value of the item to remove</param>
		/// <returns>True if the removal was successful, false otherwise</returns>
		bool RemoveItem(TKey key, TValue value);

		/// <summary>
		///     Returns an IDictionary&lt;TKey, ICollection&lt;TValue&gt;&gt; that is a copy of the one
		///     inside of this MultiDictionary. Changes to the returned IDictionary&lt;TKey, ICollection&lt;TValue&gt;&gt; will not
		///     affect this MultiDictionary.
		/// </summary>
		/// <returns>An IDictionary&lt;TKey, ICollection&lt;TValue&gt;&gt; copy of the one inside of the MultiDictionary</returns>
		IDictionary<TKey, ICollection<TValue>> ToDictionary();
	}
}