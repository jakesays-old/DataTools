using System.Collections.Generic;

namespace Std.Tools.Data.CodeModel
{
	public class Results
	{
		public string DataContextMainPart { get; set; }
		public string DataContextProcedurePart { get; set; }
		public List<Code> Models { get; private set; }

		public Results()
		{
			Models = new List<Code>();
		}
	}
}