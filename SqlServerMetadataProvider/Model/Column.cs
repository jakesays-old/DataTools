using System.Data;
using System.Diagnostics;
using LinqToDB.SchemaProvider;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class Column : Property
	{
		public ColumnSchema ColumnSchema { get; set; }
		public string ColumnName { get; set; } // Column name in database

		public bool IsNullable { get; set; }

		public bool IsIdentity { get; set; }

		// Type of the column in database
		public string ColumnType { get; set; }

		public string DataType { get; set; }

		public long? Length { get; set; }

		public int? Precision { get; set; }

		public int? Scale { get; set; }

		public DbType DbType { get; set; }

		public string Description { get; set; }

		public bool IsPrimaryKey { get; set; }

		public int PrimaryKeyOrder { get; set; }

		public bool SkipOnUpdate { get; set; }

		public bool SkipOnInsert { get; set; }

		public bool IsDuplicateOrEmpty { get; set; }

		public bool IsDiscriminator { get; set; }

		public string AliasName { get; set; }

		public string MemberName
		{
			get { return Name; }
			set { Name = value; }
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}