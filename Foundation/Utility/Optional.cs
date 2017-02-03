using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Std.Utility
{
	/// <summary>
	/// Basically Nullable{T} for both value and reference types.
	/// Tracks whether a value has been set, which makes it possible
	/// to distinguish between no value and setting a value to null,
	/// which isn't supported by references.
	/// </summary>
	/// <typeparam name="T">The type of the optional.</typeparam>
	[Serializable]
	[PublicType]
	public struct Optional<T> : IEquatable<Optional<T>>
	{
		private readonly T _value;
		private readonly bool _hasValue;

		//disabling the warning below because in this use case having
		//a static in this struct makes perfect sense.
		// ReSharper disable StaticFieldInGenericType
		[NonSerialized]
		internal static readonly bool IsNullable = typeof(T).IsNullable();

		public Optional(T value)
		{
			//disabling the warning below because in this case the IsNullable
			//check will make sure testing value for null is valid.
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (IsNullable &&
				value == null)
			{
				_hasValue = false;
				_value = default(T);
				return;
			}

			_value = value;
			_hasValue = true;
		}

		public static readonly Optional<T> NotSet = default(Optional<T>);

		public bool HasValue
		{
			get { return _hasValue; }
		}

		public T Value
		{
			get
			{
				if (!_hasValue)
				{
					throw new InvalidOperationException("Optional must have a value.");
				}
				return _value;
			}
		}

		public static explicit operator T(Optional<T> value)
		{
			return value.Value;
		}

		public static implicit operator Optional<T>(T value)
		{
			return ConvertToOptional(value);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return HasValue == false;
			}
			if (!(obj is Optional<T>))
			{
				return false;
			}
			return Equals((Optional<T>) obj);
		}

		public bool Equals(Optional<T> obj)
		{
			if (obj.HasValue != HasValue)
			{
				return false;
			}
			if (!HasValue)
			{
				return true;
			}
			return EqualityComparer<T>.Default.Equals(Value, obj.Value);
		}

		public override int GetHashCode()
		{
			if (!HasValue)
			{
				return 0;
			}
			return Value.GetHashCode();
		}

		public T GetValueOrDefault()
		{
			return GetValueOrDefault(default(T));
		}

		public T GetValueOrDefault(T defaultValue)
		{
			if (!HasValue)
			{
				return defaultValue;
			}
			return Value;
		}

		public override string ToString()
		{
			if (HasValue)
			{
				return Value.ToString();
			}
			return "";
		}

		public static bool operator ==(Optional<T> a, Optional<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Optional<T> a, Optional<T> b)
		{
			return !a.Equals(b);
		}

		public Optional<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			if (!HasValue)
			{
				return Optional<TResult>.NotSet;
			}
			var result = selector(Value);
			return ConvertToOptional(result);
		}

		public Optional<TResult> SelectMany<TCollection, TResult>(
				Func<T, Optional<TCollection>> selector,
				Func<T, TCollection, TResult> resultSelector)
		{
			if (!HasValue)
			{
				return Optional<TResult>.NotSet;
			}

			var n = selector(Value);
			if (!n.HasValue)
			{
				return Optional<TResult>.NotSet;
			}
			var result = ConvertToOptional(resultSelector(Value, n.Value));
			return result;
		}

		public static Optional<TOptional> ConvertToOptional<TOptional>(TOptional self)
		{
			//disabling the warning below because in this case the IsNullable
			//check will make sure testing value for null is valid.
			// ReSharper disable CompareNonConstrainedGenericWithNull
			if (Optional<TOptional>.IsNullable &&
				self == null)
			{
				return Optional<TOptional>.NotSet;
			}
			return new Optional<TOptional>(self);
		}
	}
}

