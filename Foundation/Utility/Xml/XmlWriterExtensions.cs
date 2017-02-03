using System;
using System.Xml;

namespace Std.Utility.Xml
{
	public static class XmlWriterExtensions
	{
		public static XmlWriter StartDocument(this XmlWriter xml, bool standalone = false)
		{
			xml.WriteStartDocument(standalone);
			return xml;
		}

		public static XmlWriter EndDocument(this XmlWriter xml)
		{
			xml.WriteEndDocument();
			return xml;
		}

		public static XmlWriter End(this XmlWriter xml)
		{
			xml.WriteEndElement();

			return xml;
		}

		public static XmlWriter Start(this XmlWriter xml, string tagName)
		{
			xml.WriteStartElement(tagName);
			return xml;
		}

		public static XmlWriter Start(this XmlWriter xml, string tagName, string namespaceName)
		{
			xml.WriteStartElement(tagName, namespaceName);
			return xml;
		}

		public static XmlWriter Element(this XmlWriter xml, string tagName, string content)
		{
			xml.WriteElementString(tagName, content);

			return xml;
		}

		public static XmlWriter Element(this XmlWriter xml, string tagName, string namespaceName, string content)
		{
			xml.WriteElementString(tagName, namespaceName, content);

			return xml;
		}

		public static XmlWriter ElementIf(this XmlWriter xml, string tagName, string content)
		{
			if (!string.IsNullOrWhiteSpace(content))
			{
				xml.WriteElementString(tagName, content);
			}

			return xml;
		}

		public static XmlWriter CData(this XmlWriter xml, string content)
		{
			xml.WriteCData(content);

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, string value)
		{
			xml.WriteAttributeString(name, value);

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, short value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, ushort value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, int value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, uint value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, long value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, ulong value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, bool value)
		{
			xml.WriteAttributeString(name, value ? "true" : "false");

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, DateTime value)
		{
			xml.WriteAttributeString(name, value.ToString("s"));

			return xml;
		}

		public static XmlWriter Attribute(this XmlWriter xml, string name, decimal value)
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		//useful for enums
		public static XmlWriter Attribute<TValueType>(this XmlWriter xml, string name, TValueType value)
			where TValueType : struct
		{
			xml.WriteAttributeString(name, value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, string value)
		{
			xml.WriteValue(value);

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, short value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, ushort value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, int value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, uint value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, long value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, ulong value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, bool value)
		{
			xml.WriteValue(value ? "true" : "false");

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, DateTime value)
		{
			xml.WriteValue(value.ToString("s"));

			return xml;
		}

		public static XmlWriter Value(this XmlWriter xml, decimal value)
		{
			xml.WriteValue(value.ToString());

			return xml;
		}

		//useful for enums
		public static XmlWriter Value<TValueType>(this XmlWriter xml, string name, TValueType value)
			where TValueType : struct
		{
			xml.WriteValue(value.ToString());

			return xml;
		}
	}
}