using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Std.Utility.Collections
{
	public static class NameValueCollectionExtensions
	{
		private static void ValidateArgs(NameValueCollection collection, string name)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}			
		}

		public static TimeSpan? GetAsTimeSpan(this NameValueCollection collection, string name)
		{
			ValidateArgs(collection, name);

			var rawValue = collection[name];
			TimeSpan value;
			if (TimeSpan.TryParse(rawValue, out value))
			{
				return value;
			}

			return null;
		}

		public static TimeSpan GetAsTimeSpan(this NameValueCollection collection, string name, TimeSpan defaultValue)
		{
			return collection.GetAsTimeSpan(name) ?? defaultValue;
		}

		public static bool? GetAsBoolean(this NameValueCollection collection, string name)
		{
			ValidateArgs(collection, name);

			var rawValue = collection[name];
			bool value;
			if (bool.TryParse(rawValue, out value))
			{
				return value;
			}

			return null;
		}

		public static bool GetAsBoolean(this NameValueCollection collection, string name, bool defaultValue)
		{
			return collection.GetAsBoolean(name) ?? defaultValue;
		}

		public static float? GetAsFloat(this NameValueCollection collection, string name)
		{
			ValidateArgs(collection, name);

			var rawValue = collection[name];
			float value;
			if (float.TryParse(rawValue, out value))
			{
				return value;
			}

			return null;
		}

		public static float GetAsFloat(this NameValueCollection collection, string name, float defaultValue)
		{
			return collection.GetAsFloat(name) ?? defaultValue;
		}

		public static int? GetAsInt(this NameValueCollection collection, string name)
		{
			ValidateArgs(collection, name);

			var rawValue = collection[name];
			int value;
			if (int.TryParse(rawValue, out value))
			{
				return value;
			}

			return null;
		}

		public static int GetAsInt(this NameValueCollection collection, string name, int defaultValue)
		{
			return collection.GetAsInt(name) ?? defaultValue;
		}

		public static long? GetAsLong(this NameValueCollection collection, string name)
		{
			ValidateArgs(collection, name);

			var rawValue = collection[name];
			long value;
			if (long.TryParse(rawValue, out value))
			{
				return value;
			}

			return null;
		}

		public static long GetAsLong(this NameValueCollection collection, string name, long defaultValue)
		{
			return collection.GetAsLong(name) ?? defaultValue;
		}

		public static TEnumType GetAsEnum<TEnumType>(this NameValueCollection collection, string name, TEnumType defaultValue)
			where TEnumType : struct
		{
			return collection.GetAsEnum<TEnumType>(name) ?? defaultValue;
		}

		public static TEnumType? GetAsEnum<TEnumType>(this NameValueCollection collection, string name)
			where TEnumType : struct
		{
			ValidateArgs(collection, name);

			if (!typeof(TEnumType).IsEnum)
			{
				throw new ArgumentException("TEnumType must be an Enum");
			}

			var valueText = collection[name];
			if (string.IsNullOrWhiteSpace(valueText))
			{
				return null;
			}

			TEnumType value;
			if (!Enum.TryParse(valueText, out value))
			{
				throw new InvalidOperationException("Cannot parse enum value");
			}

			return value;
		}

		public static string GetAsString(this NameValueCollection collection, string name, string defaultValue = null)
		{
			ValidateArgs(collection, name);

			var value = collection[name] ?? defaultValue;

			return value;
		}

		public static void AddDictionary(this NameValueCollection collection, Dictionary<string, string> source)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (source == null)
			{
				return;
			}

			foreach (var entry in source)
			{
				collection.Add(entry.Key, entry.Value);
			}
		}
	}
}
