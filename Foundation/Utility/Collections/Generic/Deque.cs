using System;
using System.Collections;
using System.Collections.Generic;

namespace Std.Utility.Collections.Generic
{
	/// <summary>
	///     A data structure much like a queue, except that you can enqueue and dequeue to both the head and the tail.
	/// </summary>
	/// <typeparam name="TElement">The type of the elements in the <see cref="Deque{T}" />.</typeparam>
	[Serializable]
	public class Deque<TElement> : ICollection<TElement>, IDeque<TElement>
	{
		private readonly LinkedList<TElement> _list;


		public Deque()
		{
			_list = new LinkedList<TElement>();
		}

		public Deque(IEnumerable<TElement> collection)
		{
			_list = new LinkedList<TElement>(collection);
		}


		public void EnqueueHead(TElement item)
		{
			EnqueueHeadItem(item);
		}

		/// <summary>
		///     Enqueues the head.
		/// </summary>
		/// <param name="item">The item.</param>
		protected virtual void EnqueueHeadItem(TElement item)
		{
			_list.AddFirst(item);
		}

		public TElement DequeueHead()
		{
			if (_list.Count == 0)
			{
				throw new InvalidOperationException("The deque is empty.");
			}


			return DequeueHeadItem();
		}

		/// <summary>
		///     Dequeues the head.
		/// </summary>
		/// <returns>The head of the deque.</returns>
		protected virtual TElement DequeueHeadItem()
		{
			var ret = _list.First.Value;
			_list.RemoveFirst();
			return ret;
		}

		public void EnqueueTail(TElement item)
		{
			EnqueueTailItem(item);
		}

		/// <summary>
		///     Enqueues the tail.
		/// </summary>
		/// <param name="item">The obj.</param>
		protected virtual void EnqueueTailItem(TElement item)
		{
			_list.AddLast(item);
		}

		public TElement DequeueTail()
		{
			if (_list.Count == 0)
			{
				throw new InvalidOperationException("The deque is empty.");
			}


			return DequeueTailItem();
		}

		/// <summary>
		///     Dequeues the tail item.
		/// </summary>
		/// <returns>The item that was dequeued.</returns>
		protected virtual TElement DequeueTailItem()
		{
			var ret = _list.Last.Value;
			_list.RemoveLast();
			return ret;
		}

		public TElement Head
		{
			get
			{
				if (_list.Count == 0)
				{
					throw new InvalidOperationException("The deque is empty.");
				}


				return _list.First.Value;
			}
		}

		public TElement Tail
		{
			get
			{
				if (_list.Count == 0)
				{
					throw new InvalidOperationException("The deque is empty.");
				}


				return _list.Last.Value;
			}
		}


		public bool Contains(TElement item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(TElement[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public void Clear()
		{
			ClearItems();
		}

		/// <summary>
		///     Clears all the objects in this instance.
		/// </summary>
		protected virtual void ClearItems()
		{
			_list.Clear();
		}

		void ICollection<TElement>.Add(TElement item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<TElement>.Remove(TElement item)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<TElement> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public bool IsEmpty
		{
			get { return Count == 0; }
		}


		public bool IsReadOnly
		{
			get { return false; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}