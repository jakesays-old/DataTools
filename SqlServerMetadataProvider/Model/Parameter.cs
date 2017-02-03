using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{ParameterName} ({ParameterType})")]

	public class Parameter : INode
	{
		public string SchemaName { get; set; }

		public string SchemaType { get; set; }

		public bool IsIn { get; set; }

		public bool IsOut { get; set; }

		public bool IsResult { get; set; }

		public long? Size { get; set; }

		public string ParameterName { get; set; }

		public string ParameterType { get; set; }

		public System.Type SystemType { get; set; }

		public string DataType { get; set; }
		public INode Parent { get; set; }

		public IEnumerable<INode> Children
		{
			get {return Enumerable.Empty<INode>(); }
		}

		public void SetParent()
		{
			
		}

		public void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}