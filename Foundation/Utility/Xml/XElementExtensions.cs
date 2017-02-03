using System;
using System.Linq;
using System.Xml.Linq;

namespace Std.Utility.Xml
{
	//convenient extensions for parsing XML via LINQ

	public static class XElementExtensions
	{
		public static string StringElement(this XElement parent, XName child)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			if (child == null)
			{
				throw new ArgumentNullException("child");
			}

			var childEl = parent.Element(child);
			if (childEl == null)
			{
				return null;
			}

			return (string) childEl;
		}

		public static bool HasAttribute(this XElement el, XName attrName)
		{
			return el.Attribute(attrName) != null;
		}

		public static bool HasAttributeValue(this XElement el, XName attrName, string value)
		{
			var attrValue = el.StringAttribute(attrName);
			return (attrValue != null) && attrValue.EqualsInvariant(value);
		}

		public static XElement FindParent(this XElement child, XName nodeName,
			bool throwIfNotFound = false)
		{
			if (child == null)
			{
				throw new ArgumentNullException("child");
			}

			if (nodeName == null)
			{
				throw new ArgumentNullException("nodeName");
			}

			XElement parent;
			while ((parent = child.Parent) != null)
			{
				if (parent.Name == nodeName)
				{
					return parent;
				}

				child = parent;
			}

			if (throwIfNotFound)
			{
				throw new InvalidOperationException(string.Format("Element '{0}' missing required parent '{1}'",
					child.Name, nodeName));
			}

			return null;
		}

		public static TEnum? EnumValue<TEnum>(this XElement el)
			where TEnum : struct
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw new InvalidOperationException("Type parameter TEnum is not valid");
			}

			TEnum vv;
			if (!Enum.TryParse(el.Value, out vv))
			{
				return null;
			}
			return vv;
		}

		public static XElement Child(this XElement parent, XName nodeName,
			bool throwIfNotFound = false, bool throwIfEmpty = false)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			if (nodeName == null)
			{
				throw new ArgumentNullException("nodeName");
			}

			var child = parent.Descendants().FirstOrDefault(c => c.Name == nodeName);

			if (child == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required child '{1}'",
						parent.Name, nodeName));
				}
				return null;
			}

			if (throwIfEmpty && child.Value.IsNullOrEmpty())
			{
				throw new InvalidOperationException(string.Format("Element '{0}' cannot be empty",
					parent.Name));
			}

			return child;
		}

		public static string ChildContent(this XElement parent, XName nodeName,
			bool throwIfNotFound = false, bool throwIfEmpty = false)
		{
			var el = parent.Child(nodeName, throwIfNotFound, throwIfEmpty);
			if (el == null)
			{
				return null;
			}

			return (string) el;
		}

		public static int IntAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return 0;
			}

			return int.Parse(attr.Value);
		}

		public static int? IntAttribute(this XElement el, XName attrName, int? defaultValue)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				return defaultValue;
			}

			return int.Parse(attr.Value);
		}

		public static long LongAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return 0;
			}

			return long.Parse(attr.Value);
		}

		public static long? LongAttribute(this XElement el, XName attrName, long? defaultValue)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				return defaultValue;
			}

			return long.Parse(attr.Value);
		}

		public static float FloatAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return 0f;
			}

			return float.Parse(attr.Value);
		}

		public static float? FloatAttribute(this XElement el, XName attrName, float? defaultValue)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				return defaultValue;
			}

			return float.Parse(attr.Value);
		}

		public static DateTime DateTimeAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null || attr.Value == "none")
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return DateTime.MinValue;
			}

			return DateTime.Parse(attr.Value);
		}

		public static TimeSpan? TimeSpanAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null || attr.Value == "none")
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return null;
			}

			return TimeSpan.Parse(attr.Value);
		}

		public static decimal DecimalAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return 0m;
			}

			return decimal.Parse(attr.Value);
		}

		public static string StringAttribute(this XElement el, XName attrName, bool throwIfNotFound = false)
		{
			var attr = el.Attribute(attrName);
			if (attr == null || string.IsNullOrEmpty(attr.Value))
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return null;
			}

			return attr.Value;
		}

		public static string StringAttribute(this XElement el, XName attrName, string defaultValue)
		{
			var attr = el.Attribute(attrName);
			if (attr == null || string.IsNullOrEmpty(attr.Value))
			{
				return defaultValue;
			}

			return attr.Value;
		}

		public static bool BoolAttribute(this XElement el, XName attrName, bool throwIfNotFound)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return false;
			}

			return bool.Parse(attr.Value);
		}

		public static bool? BoolAttribute(this XElement el, XName attrName)
		{
			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				return null;
			}

			return bool.Parse(attr.Value);
		}

		public static TEnum EnumAttribute<TEnum>(this XElement el, XName attrName, bool throwIfNotFound = false)
			where TEnum : struct
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw new InvalidOperationException("Type parameter TEnum is not valid");
			}

			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return default(TEnum);
			}

			var vv = (TEnum) Enum.Parse(typeof(TEnum), attr.Value);
			return vv;
		}

		public static object EnumAttribute(this XElement el, XName attrName, Type enumType, bool throwIfNotFound = false)
		{
			if (el == null)
			{
				throw new ArgumentNullException("el");
			}

			if (attrName == null)
			{
				throw new ArgumentNullException("attrName");
			}

			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}

			var attr = el.Attribute(attrName);
			if (attr == null)
			{
				if (throwIfNotFound)
				{
					throw new InvalidOperationException(string.Format("Element '{0}' missing required attribute '{1}'",
						el.Name, attrName.LocalName));
				}

				return null;
			}

			var vv = Enum.Parse(enumType, attr.Value);
			return vv;
		}
	}
}
