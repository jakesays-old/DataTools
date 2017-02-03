using System.Collections.Generic;

namespace Std.DataTools
{
	public class Results
	{
		public string DataContext { get; set; }
		public List<Code> Models { get; private set; }

		public Results()
		{
			Models = new List<Code>();
		}
	}
}