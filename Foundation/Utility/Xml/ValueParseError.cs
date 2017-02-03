using System;
using System.Xml;
using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public class ValueParseError : XmlParseError
	{
		private ValueParseError(string message)
			: base(message)
		{
		}

		private static string FormatMessage(XObject xml, string messagePrefix, string message)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(xml, ref line, ref col);

			var xmlDesc = "??";
			if (xml.NodeType == XmlNodeType.Element)
			{
				xmlDesc = "<{0}>".FormatWith(((XElement) xml).Name.LocalName);
			}
			else if (xml.NodeType == XmlNodeType.Attribute)
			{
				xmlDesc = "attribute " + ((XAttribute) xml).Name.LocalName;
			}

			var msg = "{0} in {1} at line {2} column {3}".FormatWith(messagePrefix, xmlDesc, line, col);
			if (!message.IsNullOrEmpty())
			{
				msg += ", message: " + message;
			}

			msg += ", xml: " + xml;

			return msg;
		}

		public static ValueParseError Create(XObject xml, Type expectedType)
		{
			var msgPrefix = "Cannot parse type '{0}'".FormatWith(expectedType.Name);
			var msg = FormatMessage(xml, msgPrefix, null);

			return new ValueParseError(msg);
		}

		public static Exception Create(XObject xml, string message)
		{
			if (xml == null)
			{
				return Create(message);
			}

			var msg = FormatMessage(xml, "Parse error", message);

			return new ValueParseError(msg);
		}

		public static Exception Create(string message)
		{
			return new ValueParseError(message);
		}
	}
}