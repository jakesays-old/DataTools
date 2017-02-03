using System;
using System.Reflection;
using System.Text;

namespace Std.Utility.Reflection
{
	public static class ReflectionHelpers
	{
		public static TAttr GetAttribute<TAttr>(MemberInfo target, bool inherit)
			where TAttr : Attribute
		{
			var attrs = target.GetCustomAttributes(typeof(TAttr), inherit) as TAttr[];
			if (attrs == null || attrs.Length == 0)
			{
				return null;
			}

			return attrs[0];
		}

		public static bool HasAttribute<TAttr>(MemberInfo target, bool inherit)
			where TAttr : Attribute
		{
			return GetAttribute<TAttr>(target, inherit) != null;
		}

		private static string FormatTypeName(Type type, NamespaceFormat namespaceFormat)
		{
			if (!type.IsGenericType)
			{
				return type.Name;
			}

			var sb = new StringBuilder();

			var gname = type.Name;
			var tildePos = gname.IndexOf('`');
			if (tildePos != -1)
			{
				gname = gname.Substring(0, tildePos);
			}

			bool first = false;
			sb.AppendFormat("{0}<", gname);
			foreach (Type arg in type.GetGenericArguments())
			{
				if (!first)
				{
					first = true;
				}
				else
				{
					sb.Append(", ");
				}

				sb.Append(GetTypeName(arg, namespaceFormat));
			}
			sb.Append(">");

			return sb.ToString();
		}

		private static bool IgnoreNamespace(Type type, NamespaceFormat namespaceFormat)
		{
			if (namespaceFormat == NamespaceFormat.All)
			{
				return false;
			}

			switch (type.Namespace)
			{
				case "System":
				case "System.Collections.Generic":
					return namespaceFormat == NamespaceFormat.ExcludeSystem;
			}

			return namespaceFormat == NamespaceFormat.None;
		}

		private static string GetTypeName(Type type, NamespaceFormat namespaceFormat)
		{
			if (!string.IsNullOrEmpty(type.Namespace) && !IgnoreNamespace(type, namespaceFormat))
			{
				return type.Namespace + "." + FormatTypeName(type, namespaceFormat);
			}

			return FormatTypeName(type, namespaceFormat);
		}

		public enum NamespaceFormat
		{
			All,
			ExcludeSystem,
			None
		}

		public static string GetMethodSignature(MethodBase method, 
			bool includeDeclaringType, bool includeParameterNames, NamespaceFormat namespaceFormat)
		{
			var sb = new StringBuilder();

			var first = false;

			if (!method.IsConstructor)
			{
				sb.Append(GetTypeName(((MethodInfo) method).ReturnType, namespaceFormat));
				sb.Append(' ');
			}

			if (includeDeclaringType)
			{
				sb.Append(GetTypeName(method.DeclaringType, namespaceFormat));
				sb.Append('.');
			}

			sb.Append(method.Name);
			sb.Append('(');

			foreach (var param in method.GetParameters())
			{
				if (!first)
				{
					first = true;
				}
				else
				{
					sb.Append(", ");
				}

				var typename = GetTypeName(param.ParameterType, namespaceFormat);
				if (typename.EndsWith("&"))
				{
					typename = typename.Substring(0, typename.Length - 1);
					sb.Append("out ");
				}

				sb.Append(typename);

				if (includeParameterNames)
				{
					sb.Append(' ');
					sb.Append(param.Name);
				}
			}
			sb.Append(")");

			var methodSignature = sb.ToString();
			return methodSignature;
		}
	}
}
