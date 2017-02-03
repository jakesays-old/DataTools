using System.Collections.Generic;
using Std.Tools.Data.Metadata;
using Std.Tools.Data.Metadata.Model;

namespace Std.Tools.Data
{
	class CollectThingsVisitor : DefaultModelVisitor
	{
		public List<Table> Tables { get; } = new List<Table>();
		public List<Class> Classes { get; } = new List<Class>();
		public List<Procedure> Procedures { get; } = new List<Procedure>();
		public List<Namespace> Namespaces { get; } = new List<Namespace>();

		public override void Visit(Class @class)
		{
			Classes.Add(@class);
		}

		public override void Visit(Table table)
		{
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

	class Program_
	{
		static void Main(string[] args)
		{
			var collector = new SqlServerMetadataCollector();

			//			collector.LoadMetadata("Server=10.66.95.246;database=GESJobDB2;User ID=EaiDbUser;Password=JdMdL..8576");
			collector.LoadMetadata("Server=10.66.95.210;database=GESJobDB;User ID=EaiDbUser;Password=JdMdL..0494");

			var flattener = new CollectThingsVisitor();
			collector.Model.Accept(flattener);
		}
	}
}
