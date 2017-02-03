using System;
using System.Collections.Generic;
using Std.Tools.Data.Metadata;
using Std.Tools.Data.Metadata.Model;

namespace Std.Tools.Data
{
	class ModelFlattener : DefaultModelVisitor
	{
		public List<Table> Tables { get; } = new List<Table>();
		public List<Class> Classes { get; } = new List<Class>();
		public List<Procedure> Procedures { get; } = new List<Procedure>();
		public List<Namespace> Namespaces { get; } = new List<Namespace>();

		public override void Visit(Class @class)
		{
			Classes.Add(@class);
		}

		private HashSet<string> _visitedTables = new HashSet<string>();

		public override void Visit(Table table)
		{
			if (_visitedTables.Contains(table.Name))
			{
				return;
			}

			_visitedTables.Add(table.Name);
			Tables.Add(table);
		}

		public override void Visit(Procedure procedure)
		{
			Procedures.Add(procedure);
		}

		public override void Visit(Namespace @namespace)
		{
			Namespaces.Add(@namespace);
		}
	}
}