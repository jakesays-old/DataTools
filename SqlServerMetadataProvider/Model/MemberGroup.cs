using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class MemberGroup : MemberBase
	{
		public string Region { get; set; }

		public bool IsCompact { get; set; }

		public bool IsPropertyGroup { get; set; }

		public List<IClassMember> Members { get; } = new List<IClassMember>();
		public List<string> Errors { get; } = new List<string>();

		public override IEnumerable<INode> Children
		{
			get { return Members; }
		}

		public override int CalcBodyLen()
		{
			return 0;
		}

		public override void SetParent()
		{
			foreach (var ch in Children)
			{
				ch.Parent = this;
				ch.SetParent();
			}
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);

			foreach (var child in Children)
			{
				child.Accept(visitor);
			}
		}
	}
}