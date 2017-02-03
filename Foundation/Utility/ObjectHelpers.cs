using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Std.Utility
{
	public static class ObjectHelpers
	{
		/// <summary>
		/// Calculate the size in bytes of an object graph rooted at <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">The root of the object graph.</param>
		/// <returns>The size in bytes of, or 0 if <paramref name="obj"/>is <code>null</code></returns>
		public static long EstimateSize(object obj)
		{
			if (obj == null)
			{
				return 0;
			}

			var type = obj.GetType();

			var size = GetSimpleTypeSize(type, obj);

			if (size > 0)
			{
				return size;
			}

			if (type.IsArray)
			{
				//handle arrays differently so we can optimize
				//for arrays of simple types
				var elType = type.GetElementType();
				size = GetSimpleTypeSize(elType);
				if (size > 0)
				{
					return size * ((Array) obj).Length;
				}

				size = ((Array) obj).Cast<object>().Sum(el => EstimateSize(el));

				return size;
			}

			if (IsIEnumerable(obj))
			{
				size = ((IEnumerable) obj).Cast<object>().Sum(el => EstimateSize(el));

				return size;
			}

			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{
				var fieldType = field.FieldType;

				//for simple types we don't need the field value
				var fieldSize = GetSimpleTypeSize(fieldType);
				if (fieldSize > 0)
				{
					size += fieldSize;
					continue;
				}

				//as an optimization we could continue to use types
				//to calculate size for fields of struct type and avoid boxing them

				var value = field.GetValue(obj);

				size += EstimateSize(value);
			}

			return size;
		}

		private static bool IsIEnumerable(object obj)
		{
			var enumObj = obj as IEnumerable;

			return enumObj != null && !enumObj.GetType().IsSimpleType();
		}

		private static long GetSimpleTypeSize(Type type, object value)
		{
			var size = GetSimpleTypeSize(type);

			if (size > 0)
			{
				return size;
			}

			if (type == typeof(string) && value != null)
			{
				return ((string) value).Length;
			}

			return 0;
		}

		private unsafe static long GetSimpleTypeSize(Type type)
		{
			if (type == typeof(bool))
			{
				return sizeof(bool);
			}

			if (type == typeof(sbyte) || type == typeof(byte))
			{
				return sizeof(sbyte);
			}

			if (type == typeof(short) || type == typeof(ushort))
			{
				return sizeof(short);
			}

			if (type == typeof(char))
			{
				return sizeof(char);
			}

			if (type == typeof(int) || type == typeof(uint))
			{
				return sizeof(int);
			}
			
			if (type == typeof(long) || type == typeof(ulong))
			{
				return sizeof(long);
			}

			if (type == typeof(float))
			{
				return sizeof(float);
			}
			
			if (type == typeof(double))
			{
				return sizeof(double);
			}

			if (type == typeof(decimal))
			{
				return sizeof(decimal);
			}

			if (type == typeof(DateTime))
			{
				return sizeof(DateTime);
			}

			if (type == typeof(DateTimeOffset))
			{
				return sizeof(DateTimeOffset);
			}

			if (type == typeof(TimeSpan))
			{
				return sizeof(TimeSpan);
			}

			if (type.IsEnum)
			{
				return GetSimpleTypeSize(Enum.GetUnderlyingType(type));
			}

			return 0;
		}
	}
}