using System.Collections.Generic;
using Std.Tools.Data.Metadata.Model;

namespace Std.Tools.Data.Metadata
{
	public interface IModelVisitor
	{
		void Visit(SchemaModel model);
		void Visit(MemberGroup memberGroup);

		void Visit(Namespace @namespace);

		void Visit(Class @class);

		void Visit(Attribute attribute);

		void Visit(Property property);
		void Visit(Column column);
		void Visit(ForeignKey key);
		void Visit(Event @event);
		void Visit(Field field);

		void Visit(Method method);
		void Visit(Procedure procedure);

		void Visit(Table table);

		void Visit(Parameter parameter);
	}
}