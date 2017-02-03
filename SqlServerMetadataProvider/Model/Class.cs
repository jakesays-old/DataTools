using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Class : Type
	{
		public string BaseClass { get; set; }
		public bool IsStatic { get; set; } = false;
		public List<string> Interfaces { get; } = new List<string>();
		public List<IClassMember> Members { get; } = new List<IClassMember>();

		public Class()
		{
		}

		public Class(string name, params IClassMember[] members)
		{
			Name = name;
			Members.AddRange(members);
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);

			foreach (var member in Members)
			{
				member.Accept(visitor);
			}
		}

		public override IEnumerable<INode> Children
		{
			get
			{
				return Members;
			}
		}

		public override void SetParent()
		{
			foreach (var ch in Children)
			{
				ch.Parent = this;
				ch.SetParent();
			}
		}
	}
}