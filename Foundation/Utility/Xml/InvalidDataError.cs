using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public class InvalidDataError : XmlParseError
	{
		private InvalidDataError(string message)
			: base(message)
		{
		}

		public static InvalidDataError Create(XElement el, XName attrName)
		{
			return Create(el, attrName, null, null);
		}

		public static InvalidDataError Create(XElement el, XName attrName, string message, params object[] args)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			var fullMessage = "Invalid data in <{0}>".FormatWith(el.Name.LocalName);
			if (attrName != null)
			{
				fullMessage += ", attribute '{0}'".FormatWith(attrName.LocalName);
			}

			fullMessage += " at line {0} column {1}".FormatWith(line, col);
			if (message != null)
			{
				fullMessage += ", message: " + message.FormatWith(args);
			}

			return new InvalidDataError(fullMessage);
		}
	}

	public class ParseError : XmlParseError
	{
		private ParseError(string message)
			: base(message)
		{
		}

		public static ParseError Create(XElement el, XName attrName, string message, params object[] args)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			var fullMessage = "Parse error in <{0}>".FormatWith(el.Name.LocalName);
			if (attrName != null)
			{
				fullMessage += ", attribute '{0}'".FormatWith(attrName.LocalName);
			}

			fullMessage += " at line {0} column {1}".FormatWith(line, col);
			if (message != null)
			{
				fullMessage += ", message: " + message.FormatWith(args);
			}

			fullMessage += ", xml: " + el;

			return new ParseError(fullMessage);
		}
	}
}