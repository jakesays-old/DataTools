using Std.Tools.Data.Metadata.Model;

namespace Std.Tools.Data.Metadata
{
	public class DefaultModelVisitor : IModelVisitor
	{
		public virtual void Visit(SchemaModel model)
		{
		}

		public virtual void Visit(MemberGroup memberGroup)
		{
		}

		public virtual void Visit(Namespace @namespace)
		{
		}

		public virtual void Visit(Class @class)
		{
		}

		public virtual void Visit(Attribute attribute)
		{
		}

		public virtual void Visit(Property property)
		{
		}

		public virtual void Visit(Column column)
		{
		}

		public virtual void Visit(ForeignKey key)
		{
		}

		public virtual void Visit(Event @event)
		{
		}

		public virtual void Visit(Field field)
		{
		}

		public virtual void Visit(Method method)
		{
		}

		public virtual void Visit(Procedure procedure)
		{
		}

		public virtual void Visit(Table table)
		{
		}

		public virtual void Visit(Parameter parameter)
		{
		}
	}
}