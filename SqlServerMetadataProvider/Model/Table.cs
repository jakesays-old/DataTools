using System.Collections.Generic;
using System.Diagnostics;
using LinqToDB.SchemaProvider;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Table : Class
	{
		public Dictionary<string, Column> Columns { get; set; }

		public List<Column> OrderedColumns { get; set; }

		public Dictionary<string, ForeignKey> ForeignKeys { get; } = new Dictionary<string, ForeignKey>();

		public TableSchema TableSchema { get; set; }

		public string Schema { get; set; }

		public string OriginalTableName { get; }

		public string TableName { get; set; }

		public string DataContextPropertyName { get; set; }

		public MemberBase DataContextProperty { get; set; }

		public bool IsView { get; set; }

		public bool IsProviderSpecific { get; set; }

		public string Description { get; set; }

		public string AliasPropertyName { get; set; }

		public string AliasTypeName { get; set; }

		public string TypePrefix { get; set; }

		public string TypeName
		{
			get { return Name; }
			set { Name = value; }
		}

		public Table(string originalTableName)
		{
			OriginalTableName = originalTableName;
		}

		public string DataObjectClassName { get; set; }
		public string PocoClassName { get; set; }
		public bool BaseClassName { get; set; }
		public bool ColumnsCleaned { get; set; }

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);

			foreach (var column in OrderedColumns)
			{
				column.Accept(visitor);
			}
		}
	}
}