using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Namespace : INode
	{
		public string Name { get; set; }

		public List<Type> Types { get; } = new List<Type>();

		public void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
			foreach (var type in Types)
			{
				type.Accept(visitor);
			}
		}

		public INode Parent { get; set; }

		public IEnumerable<INode> Children
		{
			get { return Types; }
		}

		public void SetParent()
		{
			foreach (var ch in Children)
			{
				ch.Parent = this;
				ch.SetParent();
			}
		}
	}
}