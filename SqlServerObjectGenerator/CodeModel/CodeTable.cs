using System.Collections.Generic;

namespace Std.Tools.Data.CodeModel
{
	public class CodeTable
	{
		public string Owner { get; set; }
		public string TableName { get; set; }
		public string DataObjectClassName { get; set; }
		public string PocoClassName { get; set; }
		public string DataContextPropertyName { get; set; }
		public string BaseClassName { get; set; }
		public bool IsView { get; set; }
		public string Description { get; set; }
		public bool ColumnsCleaned { get; set; }
		public List<string> Attributes = new List<string>();

		public Dictionary<string, CodeColumn> Columns = new Dictionary<string, CodeColumn>();
		public Dictionary<string, CodeForeignKey> ForeignKeys = new Dictionary<string, CodeForeignKey>();
	}
}