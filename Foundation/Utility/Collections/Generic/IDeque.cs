using System;

namespace Std.Utility.Collections.Generic
{
	/// <summary>
	/// An interface for a deque
	/// </summary>
	/// <typeparam name="TElement">The type of the elements in the deque.</typeparam>
	public interface IDeque<TElement>
	{
		/// <summary>
		/// Dequeues the head.
		/// </summary>
		/// <returns>The head of the deque.</returns>
		/// <exception cref="InvalidOperationException">The <see cref="IDeque{T}"/> is empty.</exception>
		TElement DequeueHead();

		/// <summary>
		/// Dequeues the tail.
		/// </summary>
		/// <returns>The tail of the deque.</returns>
		/// <exception cref="InvalidOperationException">The <see cref="IDeque{T}"/> is empty.</exception>
		TElement DequeueTail();

		/// <summary>
		/// Enqueues the head.
		/// </summary>
		/// <param name="item">The item.</param>
		void EnqueueHead(TElement item);

		/// <summary>
		/// Enqueues the tail.
		/// </summary>
		/// <param name="item">The item.</param>
		void EnqueueTail(TElement item);

		/// <summary>
		/// Gets the head.
		/// </summary>
		/// <value>The head.</value>
		/// <exception cref="InvalidOperationException">The <see cref="IDeque{T}"/> is empty.</exception>
		TElement Head { get; }

		/// <summary>
		/// Gets the tail.
		/// </summary>
		/// <value>The tail.</value>
		/// <exception cref="InvalidOperationException">The <see cref="IDeque{T}"/> is empty.</exception>
		TElement Tail { get; }
	}
}
