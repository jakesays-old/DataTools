using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public class InvalidElementError : XmlParseError
	{
		private InvalidElementError(string elementName, int line, int col, string message = null)
			: base(message ?? "Invalid element '{0}' found at line {1} column {2}".FormatWith(elementName, line, col))
		{
		}

		private InvalidElementError(string parentElementName, string elementName, int line, int col, string message = null)
			: base(message ?? "Invalid element '{0}' in '{1}' found at line {2} column {3}".FormatWith(parentElementName, elementName, line, col))
		{
		}

		public static InvalidElementError Create(XElement el, XName expected = null)
		{
			return InternalCreate(el.Parent, el, expected);
		}

		public static InvalidElementError Create(XElement el, string message, params object[] args)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			var messageText = "{0} (element '{1}' line {2} column {3})"
				.FormatWith(message.FormatWith(args), el.Name.LocalName, line, col);				

			return new InvalidElementError(el.Name.LocalName, line, col, messageText);
		}

		public static InvalidElementError Create(XElement parentEl, XElement el, XName expected = null)
		{
			return InternalCreate(parentEl, el, expected);
		}

		private static InvalidElementError InternalCreate(XElement parent, XElement el, XName expected)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			string message = null;
			if (parent != null)
			{
				if (expected != null)
				{
					message = "Invalid element '{0}' in '{1}' found at line {2} column {3}. Expected '{4}'"
						.FormatWith(parent.Name.LocalName, el.Name.LocalName, line, col, expected.LocalName);
				}
				return new InvalidElementError(parent.Name.LocalName, el.Name.LocalName, line, col, message);
			}

			if (expected != null)
			{
				message = "Invalid element '{0}' found at line {1} column {2}. Expected '{3}'"
					.FormatWith(el.Name.LocalName, line, col, expected.LocalName);
			}
			return new InvalidElementError(el.Name.LocalName, line, col, message);	
		}
	}
}