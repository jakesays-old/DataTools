using System.Collections.Generic;
using System.Diagnostics;

namespace Std.Tools.Data.Metadata.Model
{
	[DebuggerDisplay("{Name}")]
	public class ForeignKey : Property
	{
		private AssociationType _associationType = AssociationType.Auto;

		public ForeignKey BackReference { get; set; }

		public bool CanBeNull { get; set; }

		public string KeyName { get; set; }

		public List<Column> OtherColumns { get; set; }

		public Table OtherTable { get; set; }

		public List<Column> ThisColumns { get; set; }

		public string MemberName
		{
			get { return Name; }
			set { Name = value; }
		}

		public AssociationType AssociationType
		{
			get { return _associationType; }
			set
			{
				_associationType = value;

				if (BackReference == null)
				{
					return;
				}

				switch (value)
				{
					case AssociationType.Auto:
						BackReference.AssociationType = AssociationType.Auto;
						break;
					case AssociationType.OneToOne:
						BackReference.AssociationType = AssociationType.OneToOne;
						break;
					case AssociationType.OneToMany:
						BackReference.AssociationType = AssociationType.ManyToOne;
						break;
					case AssociationType.ManyToOne:
						BackReference.AssociationType = AssociationType.OneToMany;
						break;
				}
			}
		}

		public override void Accept(IModelVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}