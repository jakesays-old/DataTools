using System.Collections.Generic;
using System.Diagnostics;

namespace Std.DataTools
{
	[DebuggerDisplay("{_associationType} Key={KeyName} Member={MemberName} OtherTable={OtherTable.ClassName} BackRef={BackReference == null ? \"none\" : BackReference.KeyName")]
	public class CodeForeignKey
	{
		public string KeyName;
		public string MemberName;
		public CodeTable OtherTable;
		public List<CodeColumn> ThisColumns = new List<CodeColumn>();
		public List<CodeColumn> OtherColumns = new List<CodeColumn>();
		public List<string> Attributes = new List<string>();
		public bool CanBeNull = true;
		public CodeForeignKey BackReference;

		private CodeAssociationType _associationType = CodeAssociationType.Auto;

		public CodeAssociationType AssociationType
		{
			get { return _associationType; }
			set
			{
				_associationType = value;

				if (BackReference != null)
				{
					switch (value)
					{
						case CodeAssociationType.Auto:
							BackReference.AssociationType = CodeAssociationType.Auto;
							break;
						case CodeAssociationType.OneToOne:
							BackReference.AssociationType = CodeAssociationType.OneToOne;
							break;
						case CodeAssociationType.OneToMany:
							BackReference.AssociationType = CodeAssociationType.ManyToOne;
							break;
						case CodeAssociationType.ManyToOne:
							BackReference.AssociationType = CodeAssociationType.OneToMany;
							break;
					}
				}
			}
		}
	}
}