using System.Collections.Generic;

namespace Std.Tools.Data.Metadata.Model
{
	public interface INode
	{
		INode Parent { get; set; }

		IEnumerable<INode> Children { get; }

		void SetParent();

		void Accept(IModelVisitor visitor);
	}
}