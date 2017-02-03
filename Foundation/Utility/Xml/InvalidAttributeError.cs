using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public class InvalidAttributeError : XmlParseError
	{
		private InvalidAttributeError(string attributeName, int line, int col)
			: base("Invalid attribute '{0}' found at line {1} column {2}".FormatWith(attributeName, line, col))
		{
		}

		public static InvalidAttributeError Create(XElement el)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			return new InvalidAttributeError(el.Name.LocalName, line, col);
		}

		public static InvalidAttributeError Create(XElement el, XName attrName)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			return new InvalidAttributeError(attrName.LocalName, line, col);
		}
	}
}