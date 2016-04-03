using System;
using System.Collections.Generic;

namespace Twice.Models.Columns
{
	internal interface IColumnDefinitionList
	{
		event EventHandler ColumnsChanged;

		IEnumerable<ColumnDefinition> Load();

		void Save( IEnumerable<ColumnDefinition> definitions );
	}
}