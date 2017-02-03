using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Event : MemberBase
	{
		public Event()
		{
		}

		public Event(string type,
		             string name)
		{
			Type = type;
			Name = name;
		}

		public bool IsStatic { get; set; }

		public bool IsVirtual { get; set; }

		public override int CalcModifierLen()
		{
			if (IsStatic)
			{
				return " static".Length;
			}
			if (IsVirtual)
			{
				return " virtual".Length;
			}

			return " event".Length;
		}

		public override int CalcBodyLen()
		{
			return 1;
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}