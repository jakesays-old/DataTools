using System.Collections.Generic;

namespace Std.Tools.Data.Metadata.Model
{
	public interface IAttributeProvider
	{
		List<Attribute> Attributes { get; }
	}
}