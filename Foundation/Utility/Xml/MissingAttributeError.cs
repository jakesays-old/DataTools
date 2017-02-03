using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public class MissingAttributeError : XmlParseError
	{
		private MissingAttributeError(string attributeName, string elementName, int line, int col)
			: base("Missing required attribute '{0}' in <{1}> at line {2} column {3}"
				.FormatWith(attributeName, elementName, line, col))
		{
		}

		public static MissingAttributeError Create(XElement el, XName attrName)
		{
			var line = 0;
			var col = 0;
			GetLocationInfo(el, ref line, ref col);

			return new MissingAttributeError(attrName.LocalName, el.Name.LocalName, line, col);
		}
	}
}