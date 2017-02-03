using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Attribute
	{
		public string Name { get; set; }
		public List<string> Parameters { get; } = new List<string>();
		public string Conditional { get; set; }
		public bool IsSeparated { get; set; }

		public Attribute()
		{
		}

		public Attribute(string name, params string[] ps)
		{
			Name = name;
			Parameters.AddRange(ps);
		}

		public virtual void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}