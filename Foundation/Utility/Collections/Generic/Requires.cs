using System;
using System.Diagnostics;

namespace Std.Utility.Collections.Generic
{
	/// <summary>
	/// Common runtime checks that throw ArgumentExceptions upon failure.
	/// </summary>
	internal static class Requires
	{
		/// <summary>
		/// Throws an ArgumentException if a condition does not evaluate to true.
		/// </summary>
		[DebuggerStepThrough]
		public static void Argument(bool condition, string parameterName, string message)
		{
			if (!condition)
			{
				throw new ArgumentException(message, parameterName);
			}
		}

		/// <summary>
		/// Throws an ArgumentException if a condition does not evaluate to true.
		/// </summary>
		[DebuggerStepThrough]
		public static void Argument(bool condition)
		{
			if (!condition)
			{
				throw new ArgumentException();
			}
		}

		/// <summary>
		/// Throws an <see cref="T:System.ArgumentOutOfRangeException" /> if a condition does not evaluate to true.
		/// </summary>
		/// <returns>Nothing.  This method always throws.</returns>
		[DebuggerStepThrough]
		public static Exception FailRange(string parameterName, string message = null)
		{
			if (!string.IsNullOrEmpty(message))
			{
				throw new ArgumentOutOfRangeException(parameterName, message);
			}
			throw new ArgumentOutOfRangeException(parameterName);
		}

		/// <summary>
		/// Throws an exception if the specified parameter's value is null.
		/// </summary>
		/// <typeparam name="T">The type of the parameter.</typeparam>
		/// <param name="value">The value of the argument.</param>
		/// <param name="parameterName">The name of the parameter to include in any thrown exception.</param>
		/// <returns>The value of the parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c></exception>
		[DebuggerStepThrough]
		public static T NotNull<T>([ValidatedNotNull] T value, string parameterName)
		where T : class
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			return value;
		}

		/// <summary>
		/// Throws an exception if the specified parameter's value is IntPtr.Zero.
		/// </summary>
		/// <param name="value">The value of the argument.</param>
		/// <param name="parameterName">The name of the parameter to include in any thrown exception.</param>
		/// <returns>The value of the parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c></exception>
		[DebuggerStepThrough]
		public static IntPtr NotNull([ValidatedNotNull] IntPtr value, string parameterName)
		{
			if (value == IntPtr.Zero)
			{
				throw new ArgumentNullException(parameterName);
			}
			return value;
		}

		/// <summary>
		/// Throws an exception if the specified parameter's value is null.
		/// </summary>
		/// <typeparam name="T">The type of the parameter.</typeparam>
		/// <param name="value">The value of the argument.</param>
		/// <param name="parameterName">The name of the parameter to include in any thrown exception.</param>
		/// <returns>The value of the parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c></exception>
		/// <remarks>
		/// This method exists for callers who themselves only know the type as a generic parameter which
		/// may or may not be a class, but certainly cannot be null.
		/// </remarks>
		[DebuggerStepThrough]
		public static T NotNullAllowStructs<T>([ValidatedNotNull] T value, string parameterName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			return value;
		}

		/// <summary>
		/// Throws an <see cref="T:System.ArgumentOutOfRangeException" /> if a condition does not evaluate to true.
		/// </summary>
		[DebuggerStepThrough]
		public static void Range(bool condition, string parameterName, string message = null)
		{
			if (!condition)
			{
				Requires.FailRange(parameterName, message);
			}
		}
	}
}