using System.Collections.Generic;

namespace Std.Tools.Data.Metadata.Model
{
	public class SchemaModel : INode
	{
		public int CurrentNamespace { get; set; } = 0;
		public List<Namespace> Namespaces { get; } = new List<Namespace> {new Namespace()};

		public List<string> Usings { get; } = new List<string> {"System"};

		public Namespace Namespace
		{
			get { return Namespaces[CurrentNamespace]; }
		}

		public List<Type> Types
		{
			get { return Namespaces[CurrentNamespace].Types; }
		}

		public void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);

			foreach (var ns in Namespaces)
			{
				ns.Accept(visitor);
			}
		}

		public INode Parent { get; set; }

		public IEnumerable<INode> Children
		{
			get { return Namespaces; }
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