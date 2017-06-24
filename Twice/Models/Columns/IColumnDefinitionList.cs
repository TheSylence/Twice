using System;
using System.Collections.Generic;

namespace Twice.Models.Columns
{
	internal interface IColumnDefinitionList
	{
		void AddColumns( IEnumerable<ColumnDefinition> newColumns );

		IEnumerable<ColumnDefinition> Load();

		void RaiseChanged();

		void Remove( IEnumerable<ColumnDefinition> columnDefinitions );

		void Save( IEnumerable<ColumnDefinition> definitions );

		/// <summary>
		///     Informs the list about existing contexts. All definitions that belong to not existing
		///     users will be removed.
		/// </summary>
		/// <param name="ids"></param>
		void SetExistingContexts( IEnumerable<ulong> ids );

		void Update( IEnumerable<ColumnDefinition> definitions );
		event EventHandler ColumnsChanged;
	}
}