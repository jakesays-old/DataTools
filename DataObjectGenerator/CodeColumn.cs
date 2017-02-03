using System.Collections.Generic;
using System.Data;

namespace Std.DataTools
{
	public class CodeColumn
	{
		public int ID;
		public string ColumnName; // Column name in database
		public string MemberName; // Member name of the generated class
		public bool IsNullable;
		public bool IsIdentity;
		public string Type; // Type of the generated member
		public string ColumnType; // Type of the column in database
		public bool IsClass;
		public DbType DbType;
		public SqlDbType SqlDbType;
		public long Length;
		public int Precision;
		public int Scale;
		public string Description;

		public int PKIndex = -1;
		public List<string> Attributes = new List<string>();

		public bool IsPrimaryKey
		{
			get { return PKIndex >= 0; }
		}
	}
}