using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Std.Utility.Xml
{
	/// <summary>
	/// Convenient XElement/XNode selector generators.
	/// </summary>
	public static class XElementSelectors
	{
		/// <summary>
		/// Select XElements by node name
		/// </summary>
		/// <param name="reader">The source xml reader</param>
		/// <param name="elementName">Element name to filter by</param>
		/// <returns>A enumerable of XElements whose name is elementName</returns>
		public static IEnumerable<XElement> ElementNameSelector(XmlReader reader, string elementName)
		{
			reader.MoveToContent();

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element && reader.Name == elementName)
				{
					var el = XNode.ReadFrom(reader) as XElement;
					if (el != null)
					{
						yield return el;
					}
				}
			}
		}
	}
}