using System.Diagnostics.CodeAnalysis;
using Ninject;
using Ninject.Activation;
using Twice.Models.Columns;
using Twice.Utilities;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ColumnDefinitionListProvider : Provider<IColumnDefinitionList>
	{
		/// <summary>
		///     Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override IColumnDefinitionList CreateInstance( IContext context )
		{
			return new ColumnDefinitionList( Constants.IO.ColumnDefintionFileName )
			{
				Serializer = context.Kernel.Get<ISerializer>()
			};
		}
	}
}