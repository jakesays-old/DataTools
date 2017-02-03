using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Std.Utility
{
	public static class TypeExtensions
	{
		public static MethodInfo GetStaticMethod(this Type type, string methodName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (methodName.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(methodName));
			}

			var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
			return method;
		}

		public static MethodInfo GetStaticMethod(this Type type, string methodName, Type[] argTypes)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (methodName.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(methodName));
			}

			if (argTypes == null)
			{
				throw new ArgumentNullException(nameof(argTypes));
			}

			if (argTypes.Length == 0)
			{
				argTypes = null;
			}

			var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static,
				null, argTypes, null);

			return method;
		}

		/// <summary>
		/// Determines if <paramref name="type"/> implements interface <paramref name="interfaceType"/>.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <param name="interfaceType">The interface type in question.</param>
		/// <returns><code>true</code> if <paramref name="type"/> implements <paramref name="interfaceType"/></returns>
		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			
			if (interfaceType == null)
			{
				throw new ArgumentNullException(nameof(interfaceType));
			}

			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException(@"interface type required", nameof(interfaceType));
			}

			var foundTypes = type.FindInterfaces((t, _) => t == interfaceType, null);

			return foundTypes.Length > 0;
		}

		/// <summary>
		/// Determines if <paramref name="type"/>represents a simple type.
		/// Simple types are primitive types (bool, sbyte, byte, short, ushort, int, uint, long, ulong, float, double, char, decimal)
		/// and DateTime, DateTimeOffset, TimeSpan, char, string and enum.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><code>true</code> if <paramref name="type"/>is a simple type.</returns>
		public static bool IsSimpleType(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.IsSimpleValueType() || type == typeof(string);
		}

		/// <summary>
		/// Determines if <paramref name="type"/>represents a simple type.
		/// Simple types are primitive types (bool, sbyte, byte, short, ushort, int, uint, long, ulong, float, double, char, decimal)
		/// and DateTime, DateTimeOffset, TimeSpan, char and enum.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><code>true</code> if <paramref name="type"/>is a simple type.</returns>
		public static bool IsSimpleValueType(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsArray)
			{
				return false;
			}

			if (type == typeof(bool) ||
				type == typeof(sbyte) ||
				type == typeof(byte) ||
				type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(long) ||
				type == typeof(ulong) ||
				type == typeof(float) ||
				type == typeof(double) ||
				type == typeof(decimal) ||
				type == typeof(DateTime) ||
				type == typeof(DateTimeOffset) ||
				type == typeof(TimeSpan) ||
				type == typeof(char) ||
				type.IsEnum)
			{
				return true;
			}

			return false;
		}

		public static bool IsNumericValueType(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsArray)
			{
				return false;
			}

			if (type == typeof(bool) ||
				type == typeof(sbyte) ||
				type == typeof(byte) ||
				type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(long) ||
				type == typeof(ulong) ||
				type == typeof(float) ||
				type == typeof(double) ||
				type == typeof(decimal))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if <paramref name="type"/> represents a <c>System.Nullable&lt;&gt;</c> type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><code>true</code>if <paramref name="type"/> is a nullable type.</returns>
		public static bool IsNullable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!type.IsValueType && !type.IsEnum)
			{
				return false;
			}

			if (type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Return the concrete type of a Nullable{T}.
		/// </summary>
		/// <param name="type">The nullable type in question.</param>
		/// <returns>The concrete type if <paramref name="type"/> is nullable, or null otherwise.</returns>
		public static Type GetTypeOfNullable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsValueType || type.IsEnum)
			{
				if (type.IsGenericType &&
					type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return type.GetGenericArguments()[0];
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the default value for a type.
		/// </summary>
		/// <param name="type">The target type.</param>
		/// <param name="emptyCollections">For collections and arrays, 
		/// return empty collections if true, otherwise return null.</param>
		/// <returns>The default value for the target type.</returns>
		public static object GetDefaultValue(Type type, bool emptyCollections)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsEnum)
			{
				return Enum.ToObject(type, GetDefaultValue(Enum.GetUnderlyingType(type), false));
			}

			if (type.IsNullable())
			{
				return GetDefaultValue(type.GetGenericArguments()[0], false);
			}
			
			if (!type.IsSimpleValueType())
			{
				if (emptyCollections)
				{
					if (type.IsArray)
					{
						return Activator.CreateInstance(type, 0);
					}

					if (type.IsGenericType)
					{
						if (type.GetGenericTypeDefinition().ImplementsInterface(typeof(IEnumerable<>)))
						{
							return Activator.CreateInstance(type);
						}
					}
				}

				//default for reference types is null
				return null;
			}

			if (type == typeof(bool))
			{
				return false;
			}
			if (type == typeof(sbyte))
			{
				return default(sbyte);
			}
			if (type == typeof(byte))
			{
				return default(byte);
			}
			if (type == typeof(short))
			{
				return default(short);
			}
			if (type == typeof(ushort))
			{
				return default(ushort);
			}
			if (type == typeof(int))
			{
				return default(int);
			}
			if (type == typeof(uint))
			{
				return default(uint);
			}
			if (type == typeof(long))
			{
				return default(long);
			}
			if (type == typeof(ulong))
			{
				return default(ulong);
			}
			if (type == typeof(float))
			{
				return default(float);
			}
			if (type == typeof(double))
			{
				return default(double);
			}
			if (type == typeof(decimal))
			{
				return default(decimal);
			}
			if (type == typeof(DateTime))
			{
				return default(DateTime);
			}
			if (type == typeof(DateTimeOffset))
			{
				return default(DateTimeOffset);
			}
			if (type == typeof(TimeSpan))
			{
				return default(TimeSpan);
			}
			if (type == typeof(char))
			{
				return default(char);
			}

			return null;
		}

		public static string GetSimpleTypeName(this Type type)
		{
			var isNullable = type.IsNullable();
			if (!isNullable &&
				!type.IsSimpleType())
			{
				return type.Name;
			}

			var typeName = type.Name;
			if (isNullable)
			{
				typeName = type.GetTypeOfNullable().Name;
			}

			switch (typeName)
			{
				case "Int8":
					typeName = "sbyte";
					break;
				case "UInt8":
					typeName = "byte";
					break;
				case "Int16":
					typeName = "short";
					break;
				case "UInt16":
					typeName = "ushort";
					break;
				case "Int32":
					typeName = "int";
					break;
				case "UInt32":
					typeName = "uint";
					break;
				case "Int64":
					typeName = "long";
					break;
				case "UInt64":
					typeName = "ulong";
					break;
				case "Decimal":
					typeName = "decimal";
					break;
				case "Single":
					typeName = "float";
					break;
				case "Double":
					typeName = "double";
					break;
				case "DateTime":
					typeName = "DateTime";
					break;
				case "DateTimeOffset":
					typeName = "DateTimeOffset";
					break;
				case "TimeSpan":
					typeName = "TimeSpan";
					break;
				case "String":
					typeName = "string";
					break;
				case "Boolean":
					typeName = "bool";
					break;
			}

			if (isNullable)
			{
				typeName += "?";
			}

			return typeName;
		}

		public static string GetDisplayName(this Type type, bool resolveGenericArgs = true)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			var originalName = type.Name;
			var displayName = type.GetSimpleTypeName();
			if (displayName != originalName)
			{
				return displayName;
			}

			if (!type.IsGenericType &&
				!type.IsGenericTypeDefinition)
			{
				return type.Name;
			}

			if (type.IsGenericTypeDefinition ||
				!resolveGenericArgs)
			{
				displayName = "{0}<{1}>".FormatWith(type.Name.SplitOnFirst('`')[0],
					new string(',', type.GetGenericArguments().Length - 1));
			}
			else
			{
				displayName = "{0}<{1}>".FormatWith(type.Name.SplitOnFirst('`')[0],
					string.Join(",", type.GetGenericArguments().Select(t => GetDisplayName(t))));
			}

			return displayName;
		}
	}
}
