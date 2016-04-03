using Ninject.Activation;
using Twice.Models.Columns;

namespace Twice.Injections
{
	class ColumnDefinitionListProvider : Provider<IColumnDefinitionList>
	{
		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The created instance.
		/// </returns>
		protected override IColumnDefinitionList CreateInstance( IContext context )
		{
			return new ColumnDefinitionList( Constants.IO.ColumnDefintionFileName );
		}
	}
}