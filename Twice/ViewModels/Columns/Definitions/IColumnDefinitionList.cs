using System;
using System.Collections.Generic;

namespace Twice.ViewModels.Columns.Definitions
{
	internal interface IColumnDefinitionList
	{
		event EventHandler ColumnsChanged;

		IEnumerable<ColumnDefinition> Load();

		void Save( IEnumerable<ColumnDefinition> definitions );
	}
}