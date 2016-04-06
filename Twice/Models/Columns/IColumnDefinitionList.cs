using System;
using System.Collections.Generic;

namespace Twice.Models.Columns
{
	internal interface IColumnDefinitionList
	{
		event EventHandler ColumnsChanged;

		void AddColumns( IEnumerable<ColumnDefinition> newColumns );

		IEnumerable<ColumnDefinition> Load();

		void Save( IEnumerable<ColumnDefinition> definitions );
	}
}