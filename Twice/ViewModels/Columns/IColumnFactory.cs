using Twice.Models.Columns;

namespace Twice.ViewModels.Columns
{
	internal interface IColumnFactory
	{
		ColumnViewModelBase Construct( ColumnDefinition def );
	}
}