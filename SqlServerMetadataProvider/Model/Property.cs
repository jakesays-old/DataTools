using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Property : MemberBase
	{
		public List<string> GetBody = new List<string>();

		public Property()
		{
		}

		public Property(string type,
		                string name,
		                IEnumerable<string> getBody = null,
		                IEnumerable<string> setBody = null)
		{
			Type = type;
			Name = name;

			InitBody(getBody, setBody);
		}

		public int GetterLen { get; set; } = 5;
		public bool HasGetter { get; set; } = true;
		public bool HasSetter { get; set; } = true;

		public string InitValue { get; set; }

		public bool IsAbstract { get; set; }

		public bool IsAuto { get; set; } = true;

		public bool IsOverride { get; set; }

		public bool IsStatic { get; set; }

		public bool IsVirtual { get; set; }

		public List<string> SetBody { get; } = new List<string>();
		public int SetterLen { get; set; } = 5;

		public override int CalcModifierLen()
		{
			return IsVirtual
				? " virtual".Length
				: 0;
		}

		public override int CalcBodyLen()
		{
			if (IsAuto)
			{
				return 4 + GetterLen + SetterLen; // ' { get; set; }'
			}

			var len = " {".Length;

			if (HasGetter)
			{
				len += " get {".Length;
				foreach (var t in GetBody)
				{
					len += 1 + t.Length;
				}

				len += " }".Length;
			}

			if (HasSetter)
			{
				len += " set {".Length;
				foreach (var t in SetBody)
				{
					len += 1 + t.Length;
				}

				len += " }".Length;
			}

			len += " }".Length;

			return len;
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}

		public Property InitBody(IEnumerable<string> getBody = null,
		                         IEnumerable<string> setBody = null)
		{
			IsAuto = getBody == null && setBody == null;

			if (getBody != null)
			{
				GetBody.AddRange(getBody);
			}
			if (setBody != null)
			{
				SetBody.AddRange(setBody);
			}

			if (!IsAuto)
			{
				HasGetter = getBody != null;
				HasSetter = setBody != null;
			}

			return this;
		}

		public Property InitGetter(params string[] getBody)
		{
			return InitBody(getBody, null);
		}
	}
}