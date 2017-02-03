using System.Collections.Generic;
using System.Linq;

namespace Std.Tools.Data.Metadata.Model
{
	public abstract class MemberBase : IClassMember, IAttributeProvider
	{
		public string Id { get; set; }

		public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;

		public string Name { get; set; }

		public string Type { get; set; }

		public List<string> Comment { get; } = new List<string>();

		public string EndLineComment { get; set; }

		public List<Attribute> Attributes { get; } = new List<Attribute>();
		public bool InsertBlankLineAfter { get; set; } = true;

		public string Conditional { get; set; }

		public int AccessModifierLen { get; set; }

		public int ModifierLen { get; set; }

		public int TypeLen { get; set; }

		public int NameLen { get; set; }

		public int ParamLen { get; set; }

		public int BodyLen { get; set; }

		//public virtual void BeginConditional(GeneratedTextTransformation tt, bool isCompact)
		//{
		//	if (Conditional != null)
		//	{
		//		tt.RemoveSpace();
		//		tt.WriteLine("#if " + Conditional);
		//		if (!isCompact)
		//			tt.WriteLine("");
		//	}
		//}

		//public virtual void EndConditional(GeneratedTextTransformation tt, bool isCompact)
		//{
		//	if (Conditional != null)
		//	{
		//		tt.RemoveSpace();
		//		tt.WriteLine("#endif");
		//		if (!isCompact)
		//			tt.WriteLine("");
		//	}
		//}

		public INode Parent { get; set; }

		public virtual IEnumerable<INode> Children
		{
			get { return Enumerable.Empty<INode>(); }
		}

		public virtual void SetParent()
		{
		}

		public abstract void Accept(IModelVisitor visitor);

		public virtual int CalcModifierLen()
		{
			return 0;
		}

		public abstract int CalcBodyLen();

		public virtual int CalcParamLen()
		{
			return 0;
		}
	}
}