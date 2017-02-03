using System;
using System.Collections.Generic;
using System.Diagnostics;
using LinqToDB.SchemaProvider;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{ProcedureName}")]
	public class Procedure : Method
	{
		public ProcedureSchema Schema { get; set; }

		public string SchemaName { get; set; }

		public string ProcedureName { get; set; }

		public bool IsFunction { get; set; }

		public bool IsTableFunction { get; set; }

		public bool IsDefaultSchema { get; set; }

		public bool IsLoaded { get; set; }

		public Table ResultTable { get; set; }

		public Exception ResultException { get; set; }

		public List<Table> SimilarTables { get; set; }

		public List<Parameter> ProcParameters { get; set; }
		public string MethodName { get; set; }

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);

			foreach (var parameter in ProcParameters)
			{
				parameter.Accept(visitor);
			}

			ResultTable?.Accept(visitor);

			foreach (var table in SimilarTables)
			{
				table.Accept(visitor);
			}
		}
	}
}