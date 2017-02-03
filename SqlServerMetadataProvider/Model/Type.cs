using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public abstract class Type : IClassMember, IAttributeProvider
	{
		public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
		public string Name { get; set; }
		public bool IsPartial { get; set; } = true;
		public List<string> Comment { get; } = new List<string>();
		public List<Attribute> Attributes { get; } = new List<Attribute>();
		public string Conditional { get; set; }

		//protected virtual void BeginConditional(GeneratedTextTransformation tt)
		//{
		//	if (Conditional != null)
		//	{
		//		//tt.RemoveSpace();
		//		//tt.WriteLine("#if " + Conditional);
		//		//tt.WriteLine("");
		//	}
		//}

		//protected virtual void EndConditional(GeneratedTextTransformation tt)
		//{
		//	if (Conditional != null)
		//	{
		//		//tt.RemoveSpace();
		//		//tt.WriteLine("");
		//		//tt.RemoveSpace();
		//		//tt.WriteLine("#endif");
		//	}
		//}

		public INode Parent { get; set; }

		public abstract IEnumerable<INode> Children
		{
			get;
		}

		public abstract void SetParent();
		public abstract void Accept(IModelVisitor visitor);
	}
}