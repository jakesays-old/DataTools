using System;
using System.Xml;
using System.Xml.Linq;

namespace Std.Utility.Xml
{
	public abstract class XmlParseError : Exception
	{
		protected XmlParseError(string message)
			: base(message)
		{
		}

		protected static void GetLocationInfo(XObject entity, ref int line, ref int col)
		{
			line = -1;
			col = 0;
			var lineInfo = entity as IXmlLineInfo;

			if (lineInfo != null &&
				lineInfo.HasLineInfo())
			{
				line = lineInfo.LineNumber;
				col = lineInfo.LinePosition;
			}
		}
	}
}