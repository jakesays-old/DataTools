using LinqToDB.SqlProvider;

namespace Std.Tools.Data.Metadata
{
	public interface ISqlBuilderProvider
	{
		ISqlBuilder SqlBuilder { get; }
	}
}