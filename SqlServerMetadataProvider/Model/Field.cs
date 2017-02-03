using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Field : MemberBase
	{
		public Field()
		{
		}

		public Field(string type,
		             string name)
		{
			Type = type;
			Name = name;
		}

		public bool IsStatic { get; set; }

		public bool IsReadonly { get; set; }

		public string InitValue { get; set; }

		public override int CalcModifierLen()
		{
			if (IsStatic)
			{
				return " static".Length;
			}

			if (IsReadonly)
			{
				return " readonly".Length;
			}

			return 0;
		}

		public override int CalcBodyLen()
		{
			return 4 + InitValue?.Length ?? 1;
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}