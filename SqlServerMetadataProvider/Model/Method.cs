using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name} ({Type})")]
	public class Method : MemberBase
	{
		public Method()
		{
		}

		public Method(string type,
		              string name,
		              IEnumerable<string> parameters = null,
		              IEnumerable<string> body = null)
		{
			Type = type;
			Name = name;

			if (parameters != null)
			{
				Parameters.AddRange(parameters);
			}
			if (body != null)
			{
				Body.AddRange(body);
			}
		}

		public List<string> AfterSignature { get; } = new List<string>();
		public List<string> Body { get; } = new List<string>();

		public bool IsAbstract { get; set; }

		public bool IsOverride { get; set; }

		public bool IsStatic { get; set; }

		public bool IsVirtual { get; set; }

		public List<string> Parameters { get; } = new List<string>();

		public override int CalcModifierLen()
		{
			if (IsAbstract)
			{
				return " abstract".Length;
			}

			if (IsVirtual)
			{
				return " virtual".Length;
			}

			if (IsStatic)
			{
				return " static".Length;
			}

			return 0;
		}

		public override int CalcBodyLen()
		{
			if (IsAbstract || AccessModifier == AccessModifier.Partial)
			{
				return 1;
			}

			var len = " {".Length + Body.Sum(t => 1 + t.Length);

			len += " }".Length;

			return len;
		}

		public override int CalcParamLen()
		{
			return Parameters.Sum(p => p.Length + 2);
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}