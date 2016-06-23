using System;
using System.Collections.Generic;

namespace Twice.Models.Columns
{
	internal interface IColumnDefinitionList
	{
		event EventHandler ColumnsChanged;

		void AddColumns( IEnumerable<ColumnDefinition> newColumns );

		IEnumerable<ColumnDefinition> Load();

		void RaiseChanged();

		void Remove( IEnumerable<ColumnDefinition> columnDefinitions );

		void Save( IEnumerable<ColumnDefinition> definitions );

		void Update( IEnumerable<ColumnDefinition> definitions );

		/// <summary>
		/// Informs the list about existing contexts.
		/// All definitions that belong to not existing users will be removed.
		/// </summary>
		/// <param name="ids"></param>
		void SetExistingContexts( IEnumerable<ulong> ids );
	}
}